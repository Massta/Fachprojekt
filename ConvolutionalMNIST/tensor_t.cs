//========================================================================
// This conversion was produced by the Free Edition of
// C++ to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;

//C++ TO C# CONVERTER TODO TASK: The original C++ template specifier was replaced with a C# generic specifier, which may not produce the same behavior:
//ORIGINAL LINE: template<typename T>
public class tensor_t <T> : System.IDisposable
{
	public T[] data;

	public point_t size = new point_t();

	public tensor_t(int _x, int _y, int _z)
	{
		data = Arrays.InitializeWithDefaultInstances<T>(_x * _y * _z);
		size.x = _x;
		size.y = _y;
		size.z = _z;
	}

	public tensor_t(tensor_t other)
	{
		data = Arrays.InitializeWithDefaultInstances<T>(other.size.x * other.size.y * other.size.z);
//C++ TO C# CONVERTER TODO TASK: The memory management function 'memcpy' has no equivalent in C#:
		memcpy(this.data, other.data, other.size.x * other.size.y * other.size.z * sizeof(T));
//C++ TO C# CONVERTER TODO TASK: The following line was determined to be a copy assignment (rather than a reference assignment) - this should be verified and a 'CopyFrom' method should be created:
//ORIGINAL LINE: this->size = other.size;
		this.size.CopyFrom(other.size);
	}

	public static tensor_t<T> operator + (tensor_t ImpliedObject, tensor_t<T> other)
	{
		tensor_t<T> clone = new ImpliedObject.tensor_t<T>(*ImpliedObject);
		for (int i = 0; i < other.size.x * other.size.y * other.size.z; i++)
		{
			clone.data[i] += other.data[i];
		}
		return clone.functorMethod;
	}

	public static tensor_t<T> operator - (tensor_t ImpliedObject, tensor_t<T> other)
	{
		tensor_t<T> clone = new ImpliedObject.tensor_t<T>(*ImpliedObject);
		for (int i = 0; i < other.size.x * other.size.y * other.size.z; i++)
		{
			clone.data[i] -= other.data[i];
		}
		return clone.functorMethod;
	}

	public static T functorMethod(int _x, int _y, int _z)
	{
		return this.get(_x, _y, _z);
	}

	public T get(int _x, int _y, int _z)
	{
		Debug.Assert(_x >= 0 && _y >= 0 && _z >= 0);
		Debug.Assert(_x < size.x && _y < size.y && _z < size.z);

		return data[_z * (size.x * size.y) + _y * (size.x) + _x];
	}

	public void copy_from(List<List<List<T>>> data)
	{
		int z = data.Count;
		int y = data[0].Count;
		int x = data[0][0].Count;

		for (int i = 0; i < x; i++)
		{
			for (int j = 0; j < y; j++)
			{
				for (int k = 0; k < z; k++)
				{
					get(i, j, k) = data[k][j][i];
				}
			}
		}
	}

	public void Dispose()
	{
		Arrays.DeleteArray(data);
	}
}