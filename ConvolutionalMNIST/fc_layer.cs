//========================================================================
// This conversion was produced by the Free Edition of
// C++ to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;
using System.Collections.Generic;

//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma pack(push, 1)
public class fc_layer_t
{
	public layer_type type = layer_type.fc;
	public tensor_t<float> grads_in = new tensor_t<float>();
	public tensor_t<float> in = new tensor_t<float>();
	public tensor_t<float> @out = new tensor_t<float>();
	public List<float> input = new List<float>();
	public tensor_t<float> weights = new tensor_t<float>();
	public List<gradient_t> gradients = new List<gradient_t>();

	public fc_layer_t(point_t in_size, int out_size)
	{
		this.in = new tensor_t<float>(in_size.x, in_size.y, in_size.z);
		this.@out = new tensor_t<float>(out_size, 1, 1);
		this.grads_in = new tensor_t<float>(in_size.x, in_size.y, in_size.z);
		this.weights = new tensor_t<float>(in_size.x * in_size.y * in_size.z, out_size, 1);
		input = new List<float>(out_size);
		gradients = new List<gradient_t>(out_size);


		int maxval = in_size.x * in_size.y * in_size.z;

		for (int i = 0; i < out_size; i++)
		{
			for (int h = 0; h < in_size.x * in_size.y * in_size.z; h++)
			{
				weights.functorMethod(h, i, 0) = 2.19722f / maxval * RandomNumbers.NextNumber() / (float)RAND_MAX;
			}
		}
		// 2.19722f = f^-1(0.9) => x where [1 / (1 + exp(-x) ) = 0.9]
	}

	public float activator_function(float x)
	{
		//return tanhf( x );
		float sig = 1.0f / (1.0f + Math.Exp(-x));
		return sig;
	}

	public float activator_derivative(float x)
	{
		//float t = tanhf( x );
		//return 1 - t * t;
		float sig = 1.0f / (1.0f + Math.Exp(-x));
		return sig * (1 - sig);
	}

	public void activate(tensor_t<float> in)
	{
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: this->in = in;
		this.in.CopyFrom(in.functorMethod);
		activate();
	}

	public int map(point_t d)
	{
		return d.z * (in.size.x * in.size.y) + d.y * (in.size.x) + d.x;
	}

	public void activate()
	{
		for (int n = 0; n < @out.size.x; n++)
		{
			float inputv = 0F;

			for (int i = 0; i < in.size.x; i++)
			{
				for (int j = 0; j < in.size.y; j++)
				{
					for (int z = 0; z < in.size.z; z++)
					{
						int m = map({i, j, z});
						inputv += in.functorMethod(i, j, z) * weights.functorMethod(m, n, 0);
					}
				}
			}

			input[n] = inputv;

			@out.functorMethod(n, 0, 0) = activator_function(inputv);
		}
	}

	public void fix_weights()
	{
		for (int n = 0; n < @out.size.x; n++)
		{
			gradient_t grad = gradients[n];
			for (int i = 0; i < in.size.x; i++)
			{
				for (int j = 0; j < in.size.y; j++)
				{
					for (int z = 0; z < in.size.z; z++)
					{
						int m = map({i, j, z});
//C++ TO C# CONVERTER TODO TASK: C# does not have an equivalent to references to value types:
//ORIGINAL LINE: float& w = weights(m, n, 0);
						float w = weights.functorMethod(m, n, 0);
						w = update_weight(w, grad, in.functorMethod(i, j, z));
					}
				}
			}

			update_gradient(grad);
		}
	}

	public void calc_grads(tensor_t<float> grad_next_layer)
	{
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memset' has no equivalent in C#:
		memset(grads_in.data, 0, grads_in.size.x * grads_in.size.y * grads_in.size.z * sizeof(float));
		for (int n = 0; n < @out.size.x; n++)
		{
			gradient_t grad = gradients[n];
//C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
//ORIGINAL LINE: grad.grad = grad_next_layer(n, 0, 0) * activator_derivative(input[n]);
			grad.grad = grad_next_layer.functorMethod(n, 0, 0) * activator_derivative(new List(input[n]));

			for (int i = 0; i < in.size.x; i++)
			{
				for (int j = 0; j < in.size.y; j++)
				{
					for (int z = 0; z < in.size.z; z++)
					{
						int m = map({i, j, z});
						grads_in.functorMethod(i, j, z) += grad.grad * weights.functorMethod(m, n, 0);
					}
				}
			}
		}
	}
}
//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma pack(pop)
