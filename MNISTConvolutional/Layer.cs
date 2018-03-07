using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MNISTConvolutional
{
    public abstract class Layer
    {
        public LayerType Type { get; set; }
        public Tensor GradsIn { get; set; }
        public Tensor In { get; set; }
        public Tensor Out { get; set; }
        public abstract void Activate();
        public abstract void Activate(Tensor input);
        public abstract void CalculateGradients(Tensor input);
        public abstract void FixWeights();
    }


    public enum LayerType
    {
        Convolutional,
        FullyConnected,
        ReLU,
        Pooling,
        Dropout
    }
}
