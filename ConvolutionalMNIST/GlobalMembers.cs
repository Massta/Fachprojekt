//========================================================================
// This conversion was produced by the Free Edition of
// C++ to C# Converter courtesy of Tangible Software Solutions.
// Order the Premium Edition at https://www.tangiblesoftwaresolutions.com
//========================================================================

public static class GlobalMembers
{

	internal static void calc_grads(layer_t layer, tensor_t<float> grad_next_layer)
	{
		switch (layer.type)
		{
			case layer_type.conv:
				((conv_layer_t)layer).calc_grads(grad_next_layer.functorMethod);
				return;
			case layer_type.relu:
				((relu_layer_t)layer).calc_grads(grad_next_layer.functorMethod);
				return;
			case layer_type.fc:
				((fc_layer_t)layer).calc_grads(grad_next_layer.functorMethod);
				return;
			case layer_type.pool:
				((pool_layer_t)layer).calc_grads(grad_next_layer.functorMethod);
				return;
			case layer_type.dropout_layer:
				((dropout_layer_t)layer).calc_grads(grad_next_layer.functorMethod);
				return;
			default:
				Debug.Assert(false);
			break;
		}
	}

	internal static void fix_weights(layer_t layer)
	{
		switch (layer.type)
		{
			case layer_type.conv:
				((conv_layer_t)layer).fix_weights();
				return;
			case layer_type.relu:
				((relu_layer_t)layer).fix_weights();
				return;
			case layer_type.fc:
				((fc_layer_t)layer).fix_weights();
				return;
			case layer_type.pool:
				((pool_layer_t)layer).fix_weights();
				return;
			case layer_type.dropout_layer:
				((dropout_layer_t)layer).fix_weights();
				return;
			default:
				Debug.Assert(false);
			break;
		}
	}

	internal static void activate(layer_t layer, tensor_t<float> in)
	{
		switch (layer.type)
		{
			case layer_type.conv:
				((conv_layer_t)layer).activate(in.functorMethod);
				return;
			case layer_type.relu:
				((relu_layer_t)layer).activate(in.functorMethod);
				return;
			case layer_type.fc:
				((fc_layer_t)layer).activate(in.functorMethod);
				return;
			case layer_type.pool:
				((pool_layer_t)layer).activate(in.functorMethod);
				return;
			case layer_type.dropout_layer:
				((dropout_layer_t)layer).activate(in.functorMethod);
				return;
			default:
				Debug.Assert(false);
			break;
		}
	}

	internal static float update_weight(float w, gradient_t grad, float multp = 1)
	{
		float m = (grad.grad + grad.oldgrad * DefineConstants.MOMENTUM);
		w -= DefineConstants.LEARNING_RATE * m * multp + DefineConstants.LEARNING_RATE * DefineConstants.WEIGHT_DECAY * w;
		return w;
	}

	internal static void update_gradient(gradient_t grad)
	{
		grad.oldgrad = (grad.grad + grad.oldgrad * DefineConstants.MOMENTUM);
	}

	internal static void print_tensor(tensor_t<float> data)
	{
		int mx = data.size.x;
		int my = data.size.y;
		int mz = data.size.z;

		for (int z = 0; z < mz; z++)
		{
			Console.Write("[Dim{0:D}]\n", z);
			for (int y = 0; y < my; y++)
			{
				for (int x = 0; x < mx; x++)
				{
					Console.Write("{0:f2} \t", (float)data.get(x, y, z));
				}
				Console.Write("\n");
			}
		}
	}

	internal static tensor_t<float> to_tensor(List<List<List<float>>> data)
	{
		int z = data.Count;
		int y = data[0].Count;
		int x = data[0][0].Count;


		tensor_t<float> t = new tensor_t<float>(x, y, z);

		for (int i = 0; i < x; i++)
		{
			for (int j = 0; j < y; j++)
			{
				for (int k = 0; k < z; k++)
				{
					t.functorMethod(i, j, k) = data[k][j][i];
				}
			}
		}
		return t.functorMethod;
    }


    public static float train(vector<layer_t> layers, tensor_t<float> data, tensor_t<float> expected)
    {
        for (int i = 0; i < layers.size(); i++)
        {
            if (i == 0)
            {
                activate(layers[i], data.functorMethod);
            }
            else
            {
                activate(layers[i], layers[i - 1].@out);
            }
        }

        tensor_t<float> grads = layers.back().@out - expected.functorMethod;

        for (int i = layers.size() - 1; i >= 0; i--)
        {
            if (i == layers.size() - 1)
            {
                calc_grads(layers[i], grads.functorMethod);
            }
            else
            {
                calc_grads(layers[i], layers[i + 1].grads_in);
            }
        }

        for (int i = 0; i < layers.size(); i++)
        {
            fix_weights(layers[i]);
        }

        float err = 0F;
        for (int i = 0; i < grads.size.x * grads.size.y * grads.size.z; i++)
        {
            float f = expected.data[i];
            if (f > 0.5F)
            {
                err += Math.Abs(grads.data[i]);
            }
        }
        return err * 100;
    }


    public static void forward(vector<layer_t> layers, tensor_t<float> data)
    {
        for (int i = 0; i < layers.size(); i++)
        {
            if (i == 0)
            {
                activate(layers[i], data.functorMethod);
            }
            else
            {
                activate(layers[i], layers[i - 1].@out);
            }
        }
    }

    public static uint8_t read_file(string szFile)
    {
        ifstream file = new ifstream(szFile, ios.binary | ios.ate);
        streamsize size = file.tellg();
        file.seekg(0, ios.beg);

        if (size == -1)
        {
            return null;
        }

        uint8_t[] buffer = Arrays.InitializeWithDefaultInstances<uint8_t>(size);
        file.read((string)buffer, size);
        return buffer;
    }

    public static vector<case_t> read_test_cases()
    {
        vector<case_t> cases = new vector<case_t>();

        uint8_t train_image = read_file("train-images.idx3-ubyte");
        uint8_t train_labels = read_file("train-labels.idx1-ubyte");

        uint32_t case_count = byteswap_uint32((uint32_t)(train_image + 4));

        for (int i = 0; i < case_count; i++)
        {
            case_t c = new case_t(new tensor_t<float>(28, 28, 1), new tensor_t<float>(10, 1, 1));

            uint8_t[] img = train_image + 16 + i * (28 * 28);
            uint8_t label = train_labels + 8 + i;

            for (int x = 0; x < 28; x++)
            {
                for (int y = 0; y < 28; y++)
                {
                    c.data.functorMethod(x, y, 0) = img[x + y * 28] / 255.0f;
                }
            }

            for (int b = 0; b < 10; b++)
            {
                c.@out.functorMethod(b, 0, 0) = label == b != 0 ? 1.0f : 0.0f;
            }

            cases.push_back(c);
        }
        train_image = null;
        train_labels = null;

        return cases;
    }

    static int Main()
    {
        vector<case_t> cases = read_test_cases();

        vector<layer_t> layers = new vector<layer_t>();

        conv_layer_t layer1 = new conv_layer_t(1, 5, 8, cases[0].data.size); // 28 * 28 * 1 -> 24 * 24 * 8
                                                                             //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
                                                                             //ORIGINAL LINE: relu_layer_t * layer2 = new relu_layer_t(layer1->out.size);
        relu_layer_t layer2 = new relu_layer_t(new point_t(layer1.@out.size));
        //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
        //ORIGINAL LINE: pool_layer_t * layer3 = new pool_layer_t(2, 2, layer2->out.size);
        pool_layer_t layer3 = new pool_layer_t(2, 2, new point_t(layer2.@out.size)); // 24 * 24 * 8 -> 12 * 12 * 8
                                                                                     //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
                                                                                     //ORIGINAL LINE: fc_layer_t * layer4 = new fc_layer_t(layer3->out.size, 10);
        fc_layer_t layer4 = new fc_layer_t(new point_t(layer3.@out.size), 10); // 4 * 4 * 16 -> 10

        layers.push_back((layer_t)layer1);
        layers.push_back((layer_t)layer2);
        layers.push_back((layer_t)layer3);
        layers.push_back((layer_t)layer4);



        float amse = 0F;
        int ic = 0;

        for (int ep = 0; ep < 100000;)
        {

            foreach (case_t t in cases)
            {
                float xerr = train(layers, t.data.functorMethod, t.@out.functorMethod);
                amse += xerr;

                ep++;
                ic++;

                if (ep % 1000 == 0)
                {
                    Console.Write("case ");
                    Console.Write(ep);
                    Console.Write(" err=");
                    Console.Write(amse / ic);
                    Console.Write("\n");
                }

                // if ( GetAsyncKeyState( VK_F1 ) & 0x8000 )
                // {
                //	   printf( "err=%.4f%\n", amse / ic  );
                //	   goto end;
                // }
            }
        }
        // end:



        while (true)
        {
            uint8_t[] data = read_file("test.ppm");

            if (data != null)
            {
                //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
                //ORIGINAL LINE: uint8_t * usable = data;
                uint8_t[] usable = new uint8_t(data);

                while ((uint32_t)usable != 0x0A353532)
                {
                    usable++;
                }

                //C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
                //#pragma pack(push, 1)
                //C++ TO C# CONVERTER TODO TASK: C# does not allow declaring types within methods:
                //			struct RGB
                //			{
                //				uint8_t r, g, b;
                //			};
                //C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
                //#pragma pack(pop)

                RGB[] rgb = (RGB)usable;

                tensor_t<float> image = new tensor_t<float>(28, 28, 1);
                for (int i = 0; i < 28; i++)
                {
                    for (int j = 0; j < 28; j++)
                    {
                        RGB rgb_ij = rgb[i * 28 + j];
                        image.functorMethod(j, i, 0) = (((float)rgb_ij.r + rgb_ij.g + rgb_ij.b) / (3.0f * 255.0f));
                    }
                }

                forward(layers, image.functorMethod);
                tensor_t<float> @out = layers.back().@out;
                for (int i = 0; i < 10; i++)
                {
                    Console.Write("[{0:D}] {1:f}\n", i, @out.functorMethod(i, 0, 0) * 100.0f);
                }

                data = null;
            }

            timespec wait = new timespec();
            wait.tv_sec = 1;
            wait.tv_nsec = 0;
            nanosleep(wait, null);
        }
        return 0;
    }



    public static float train(vector<layer_t> layers, tensor_t<float> data, tensor_t<float> expected)
    {
        for (int i = 0; i < layers.size(); i++)
        {
            if (i == 0)
            {
                activate(layers[i], data.functorMethod);
            }
            else
            {
                activate(layers[i], layers[i - 1].@out);
            }
        }

        tensor_t<float> grads = layers.back().@out - expected.functorMethod;

        for (int i = layers.size() - 1; i >= 0; i--)
        {
            if (i == layers.size() - 1)
            {
                calc_grads(layers[i], grads.functorMethod);
            }
            else
            {
                calc_grads(layers[i], layers[i + 1].grads_in);
            }
        }

        for (int i = 0; i < layers.size(); i++)
        {
            fix_weights(layers[i]);
        }

        float err = 0F;
        for (int i = 0; i < grads.size.x * grads.size.y * grads.size.z; i++)
        {
            float f = expected.data[i];
            if (f > 0.5F)
            {
                err += Math.Abs(grads.data[i]);
            }
        }
        return err * 100;
    }


    public static void forward(vector<layer_t> layers, tensor_t<float> data)
    {
        for (int i = 0; i < layers.size(); i++)
        {
            if (i == 0)
            {
                activate(layers[i], data.functorMethod);
            }
            else
            {
                activate(layers[i], layers[i - 1].@out);
            }
        }
    }

    public static uint8_t read_file(string szFile)
    {
        ifstream file = new ifstream(szFile, ios.binary | ios.ate);
        streamsize size = file.tellg();
        file.seekg(0, ios.beg);

        if (size == -1)
        {
            return null;
        }

        uint8_t[] buffer = Arrays.InitializeWithDefaultInstances<uint8_t>(size);
        file.read((string)buffer, size);
        return buffer;
    }

    public static vector<case_t> read_test_cases()
    {
        vector<case_t> cases = new vector<case_t>();

        uint8_t train_image = read_file("train-images.idx3-ubyte");
        uint8_t train_labels = read_file("train-labels.idx1-ubyte");

        uint32_t case_count = byteswap_uint32((uint32_t)(train_image + 4));

        for (int i = 0; i < case_count; i++)
        {
            case_t c = new case_t(new tensor_t<float>(28, 28, 1), new tensor_t<float>(10, 1, 1));

            uint8_t[] img = train_image + 16 + i * (28 * 28);
            uint8_t label = train_labels + 8 + i;

            for (int x = 0; x < 28; x++)
            {
                for (int y = 0; y < 28; y++)
                {
                    c.data.functorMethod(x, y, 0) = img[x + y * 28] / 255.0f;
                }
            }

            for (int b = 0; b < 10; b++)
            {
                c.@out.functorMethod(b, 0, 0) = label == b != 0 ? 1.0f : 0.0f;
            }

            cases.push_back(c);
        }
        train_image = null;
        train_labels = null;

        return cases;
    }

    static int Main()
    {

        vector<case_t> cases = read_test_cases();

        vector<layer_t> layers = new vector<layer_t>();

        conv_layer_t layer1 = new conv_layer_t(1, 5, 8, cases[0].data.size); // 28 * 28 * 1 -> 24 * 24 * 8
                                                                             //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
                                                                             //ORIGINAL LINE: relu_layer_t * layer2 = new relu_layer_t(layer1->out.size);
        relu_layer_t layer2 = new relu_layer_t(new point_t(layer1.@out.size));
        //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
        //ORIGINAL LINE: pool_layer_t * layer3 = new pool_layer_t(2, 2, layer2->out.size);
        pool_layer_t layer3 = new pool_layer_t(2, 2, new point_t(layer2.@out.size)); // 24 * 24 * 8 -> 12 * 12 * 8

        //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
        //ORIGINAL LINE: conv_layer_t * layer4 = new conv_layer_t(1, 3, 10, layer3->out.size);
        conv_layer_t layer4 = new conv_layer_t(1, 3, 10, new point_t(layer3.@out.size)); // 12 * 12 * 6 -> 10 * 10 * 10
                                                                                         //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
                                                                                         //ORIGINAL LINE: relu_layer_t * layer5 = new relu_layer_t(layer4->out.size);
        relu_layer_t layer5 = new relu_layer_t(new point_t(layer4.@out.size));
        //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
        //ORIGINAL LINE: pool_layer_t * layer6 = new pool_layer_t(2, 2, layer5->out.size);
        pool_layer_t layer6 = new pool_layer_t(2, 2, new point_t(layer5.@out.size)); // 10 * 10 * 10 -> 5 * 5 * 10

        //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
        //ORIGINAL LINE: fc_layer_t * layer7 = new fc_layer_t(layer6->out.size, 10);
        fc_layer_t layer7 = new fc_layer_t(new point_t(layer6.@out.size), 10); // 4 * 4 * 16 -> 10

        layers.push_back((layer_t)layer1);
        layers.push_back((layer_t)layer2);
        layers.push_back((layer_t)layer3);
        layers.push_back((layer_t)layer4);
        layers.push_back((layer_t)layer5);
        layers.push_back((layer_t)layer6);
        layers.push_back((layer_t)layer7);



        float amse = 0F;
        int ic = 0;

        for (int ep = 0; ep < 100000;)
        {

            foreach (case_t t in cases)
            {
                float xerr = train(layers, t.data.functorMethod, t.@out.functorMethod);
                amse += xerr;

                ep++;
                ic++;

                if (ep % 1000 == 0)
                {
                    Console.Write("case ");
                    Console.Write(ep);
                    Console.Write(" err=");
                    Console.Write(amse / ic);
                    Console.Write("\n");
                }

                // if ( GetAsyncKeyState( VK_F1 ) & 0x8000 )
                // {
                //     printf( "err=%.4f%\n", amse / ic  );
                //     goto end;
                // }
            }
        }
        // end:



        while (true)
        {
            uint8_t[] data = read_file("test.ppm");

            if (data != null)
            {
                //C++ TO C# CONVERTER TODO TASK: The following line was determined to contain a copy constructor call - this should be verified and a copy constructor should be created:
                //ORIGINAL LINE: uint8_t * usable = data;
                uint8_t[] usable = new uint8_t(data);

                while ((uint32_t)usable != 0x0A353532)
                {
                    usable++;
                }

                //C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
                //#pragma pack(push, 1)
                //C++ TO C# CONVERTER TODO TASK: C# does not allow declaring types within methods:
                //			struct RGB
                //			{
                //				uint8_t r, g, b;
                //			};
                //C++ TO C# CONVERTER TODO TASK: There is no equivalent to most C++ 'pragma' directives in C#:
                //#pragma pack(pop)

                RGB[] rgb = (RGB)usable;

                tensor_t<float> image = new tensor_t<float>(28, 28, 1);
                for (int i = 0; i < 28; i++)
                {
                    for (int j = 0; j < 28; j++)
                    {
                        RGB rgb_ij = rgb[i * 28 + j];
                        image.functorMethod(j, i, 0) = ((((float)rgb_ij.r + rgb_ij.g + rgb_ij.b) / (3.0f * 255.0f)));
                    }
                }

                forward(layers, image.functorMethod);
                tensor_t<float> @out = layers.back().@out;
                for (int i = 0; i < 10; i++)
                {
                    Console.Write("[{0:D}] {1:f}\n", i, @out.functorMethod(i, 0, 0) * 100.0f);
                }

                data = null;
            }

            timespec wait = new timespec();
            wait.tv_sec = 1;
            wait.tv_nsec = 0;
            nanosleep(wait, null);
        }
        return 0;
    }

    public static uint32_t byteswap_uint32(uint32_t a)
    {
        return ((((a >> 24) & 0xff) << 0) | (((a >> 16) & 0xff) << 8) | (((a >> 8) & 0xff) << 16) | (((a >> 0) & 0xff) << 24));
    }

#if _WIN32 || _WIN64

	public static void nanosleep(timespec req, timespec rem)
	{
		Sleep((uint)(req.tv_sec * 1000) + (uint)(req.tv_nsec != 0 ? req.tv_nsec / 1000000 : 0));
	}
#endif

}