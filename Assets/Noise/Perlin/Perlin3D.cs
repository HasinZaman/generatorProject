using System;

public class Perlin3D : Perlin<Perlin3D.Vector3DNode>
{
    public class Vector3DNode : VectorNode
    {
        /// <summary>
        ///     neighbor node left relative to the current node
        /// </summary>
        public Vector3DNode left = null;

        /// <summary>
        ///     neighbor node right relative to the current node
        /// </summary>
        public Vector3DNode right = null;

        /// <summary>
        ///     neighbor node up relative to the current node
        /// </summary>
        public Vector3DNode up = null;

        /// <summary>
        ///     neighbor node down relative to the current node
        /// </summary>
        public Vector3DNode down = null;

        /// <summary>
        ///     neighbor node above relative to the current node
        /// </summary>
        public Vector3DNode above = null;

        /// <summary>
        ///     neighbor node below relative to the current node
        /// </summary>
        public Vector3DNode below = null;

        public Vector3DNode(float[] vector)
        {
            this.dim = 3;

            this.set(vector);
        }
    }
    }
}
