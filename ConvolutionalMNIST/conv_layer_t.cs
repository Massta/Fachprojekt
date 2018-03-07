//========================================================================
// This conversion was produced by the Free Edition of
// C++ to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma pack(push, 1)
public class conv_layer_t
{
	public layer_type type = layer_type.conv;
	public tensor_t<float> grads_in = new tensor_t<float>();
	public tensor_t<float> in = new tensor_t<float>();
	public tensor_t<float> @out = new tensor_t<float>();
	public List<tensor_t<float>> filters = new List<tensor_t<float>>();
	public List<tensor_t<gradient_t>> filter_grads = new List<tensor_t<gradient_t>>();
	public uint16_t stride = new uint16_t();
	public uint16_t extend_filter = new uint16_t();

	public conv_layer_t(uint16_t stride, uint16_t extend_filter, uint16_t number_filters, point_t in_size)

	{
		this.grads_in = new tensor_t<float>(in_size.x, in_size.y, in_size.z);
		this.in = new tensor_t<float>(in_size.x, in_size.y, in_size.z);
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: this.out = new tensor_t<float>((in_size.x - extend_filter) / stride + 1, (in_size.y - extend_filter) / stride + 1, number_filters);
		this.@out = new tensor_t<float>((in_size.x - extend_filter) / stride + 1, (in_size.y - extend_filter) / stride + 1, new uint16_t(number_filters));
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: this->stride = stride;
		this.stride.CopyFrom(stride);
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: this->extend_filter = extend_filter;
		this.extend_filter.CopyFrom(extend_filter);
		Debug.Assert(((float)(in_size.x - extend_filter) / stride + 1) == ((in_size.x - extend_filter) / stride + 1));

		Debug.Assert(((float)(in_size.y - extend_filter) / stride + 1) == ((in_size.y - extend_filter) / stride + 1));

		for (int a = 0; a < number_filters; a++)
		{
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: tensor_t<float> t(extend_filter, extend_filter, in_size.z);
			tensor_t<float> t = new tensor_t<float>(new uint16_t(extend_filter), new uint16_t(extend_filter), in_size.z);

			int maxval = extend_filter * extend_filter * in_size.z;

			for (int i = 0; i < extend_filter; i++)
			{
				for (int j = 0; j < extend_filter; j++)
				{
					for (int z = 0; z < in_size.z; z++)
					{
						t.functorMethod(i, j, z) = 1.0f / maxval * RandomNumbers.NextNumber() / (float)RAND_MAX;
					}
				}
			}
			filters.Add(t.functorMethod);
		}
		for (int i = 0; i < number_filters; i++)
		{
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: tensor_t<gradient_t> t(extend_filter, extend_filter, in_size.z);
			tensor_t<gradient_t> t = new tensor_t<gradient_t>(new uint16_t(extend_filter), new uint16_t(extend_filter), in_size.z);
			filter_grads.Add(t.functorMethod);
		}

	}

	public point_t map_to_input(point_t @out, int z)
	{
		@out.x *= stride;
		@out.y *= stride;
		@out.z = z;
		return @out;
	}

	public class range_t
	{
		public int min_x;
		public int min_y;
		public int min_z;
		public int max_x;
		public int max_y;
		public int max_z;
	}

	public int normalize_range(float f, int max, bool lim_min)
	{
		if (f <= 0F)
		{
			return 0;
		}
		max -= 1;
		if (f >= max)
		{
			return max;
		}

		if (lim_min) // left side of inequality
		{
			return Math.Ceiling(f);
		}
		else
		{
			return Math.Floor(f);
		}
	}

	public range_t map_to_output(int x, int y)
	{
		float a = x;
		float b = y;
		return {normalize_range((a - extend_filter + 1) / stride, @out.size.x, true), normalize_range((b - extend_filter + 1) / stride, @out.size.y, true), 0, normalize_range(a / stride, @out.size.x, false), normalize_range(b / stride, @out.size.y, false), (int)filters.Count - 1};
	}

	public void activate(tensor_t<float> in)
	{
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: this->in = in;
		this.in.CopyFrom(in.functorMethod);
		activate();
	}

	public void activate()
	{
		for (int filter = 0; filter < filters.Count; filter++)
		{
			tensor_t<float> filter_data = filters[filter].functorMethod;
			for (int x = 0; x < @out.size.x; x++)
			{
				for (int y = 0; y < @out.size.y; y++)
				{
					point_t mapped = map_to_input({(uint16_t)x, (uint16_t)y, 0}, 0);
					float sum = 0F;
					for (int i = 0; i < extend_filter; i++)
					{
						for (int j = 0; j < extend_filter; j++)
						{
							for (int z = 0; z < in.size.z; z++)
							{
								float f = filter_data.functorMethod(i, j, z);
								float v = in.functorMethod(mapped.x + i, mapped.y + j, z);
								sum += f * v;
							}
						}
					}
					@out.functorMethod(x, y, filter) = sum;
				}
			}
		}
	}

	public void fix_weights()
	{
		for (int a = 0; a < filters.Count; a++)
		{
			for (int i = 0; i < extend_filter; i++)
			{
				for (int j = 0; j < extend_filter; j++)
				{
					for (int z = 0; z < in.size.z; z++)
					{
//C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent to references to value types:
//ORIGINAL LINE: float& w = filters[a].get(i, j, z);
						float w = filters[a].get(i, j, z);
						gradient_t grad = filter_grads[a].get(i, j, z);
						w = update_weight(w, grad);
						update_gradient(grad);
					}
				}
			}
		}
	}

	public void calc_grads(tensor_t<float> grad_next_layer)
	{

		for (int k = 0; k < filter_grads.Count; k++)
		{
			for (int i = 0; i < extend_filter; i++)
			{
				for (int j = 0; j < extend_filter; j++)
				{
					for (int z = 0; z < in.size.z; z++)
					{
						filter_grads[k].get(i, j, z).grad = 0;
					}
				}
			}
		}

		for (int x = 0; x < in.size.x; x++)
		{
			for (int y = 0; y < in.size.y; y++)
			{
				range_t rn = map_to_output(x, y);
				for (int z = 0; z < in.size.z; z++)
				{
					float sum_error = 0F;
					for (int i = rn.min_x; i <= rn.max_x; i++)
					{
						int minx = i * stride;
						for (int j = rn.min_y; j <= rn.max_y; j++)
						{
							int miny = j * stride;
							for (int k = rn.min_z; k <= rn.max_z; k++)
							{
								int w_applied = filters[k].get(x - minx, y - miny, z);
								sum_error += w_applied * grad_next_layer.functorMethod(i, j, k);
								filter_grads[k].get(x - minx, y - miny, z).grad += in.functorMethod(x, y, z) * grad_next_layer.functorMethod(i, j, k);
							}
						}
					}
					grads_in.functorMethod(x, y, z) = sum_error;
				}
			}
		}
	}
}
//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma pack(pop)