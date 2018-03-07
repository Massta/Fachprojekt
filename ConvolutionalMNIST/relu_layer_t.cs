//========================================================================
// This conversion was produced by the Free Edition of
// C++ to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma pack(push, 1)
public class relu_layer_t
{
	public layer_type type = layer_type.relu;
	public tensor_t<float> grads_in = new tensor_t<float>();
	public tensor_t<float> in = new tensor_t<float>();
	public tensor_t<float> @out = new tensor_t<float>();

	public relu_layer_t(point_t in_size)
	{
		this.in = new tensor_t<float>(in_size.x, in_size.y, in_size.z);
		this.@out = new tensor_t<float>(in_size.x, in_size.y, in_size.z);
		this.grads_in = new tensor_t<float>(in_size.x, in_size.y, in_size.z);
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
		for (int i = 0; i < in.size.x; i++)
		{
			for (int j = 0; j < in.size.y; j++)
			{
				for (int z = 0; z < in.size.z; z++)
				{
					float v = in.functorMethod(i, j, z);
					if (v < 0F)
					{
						v = 0F;
					}
					@out.functorMethod(i, j, z) = v;
				}
			}
		}

	}

	public void fix_weights()
	{

	}

	public void calc_grads(tensor_t<float> grad_next_layer)
	{
		for (int i = 0; i < in.size.x; i++)
		{
			for (int j = 0; j < in.size.y; j++)
			{
				for (int z = 0; z < in.size.z; z++)
				{
					grads_in.functorMethod(i, j, z) = (in.functorMethod(i, j, z) < 0) ? (0) : (1 * grad_next_layer.functorMethod(i, j, z));
				}
			}
		}
	}
}
//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma pack(pop)