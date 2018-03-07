//========================================================================
// This conversion was produced by the Free Edition of
// C++ to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma pack(push, 1)
public class layer_t
{
	public layer_type type;
	public tensor_t<float> grads_in = new tensor_t<float>();
	public tensor_t<float> in = new tensor_t<float>();
	public tensor_t<float> @out = new tensor_t<float>();
}
//C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
//#pragma pack(pop)