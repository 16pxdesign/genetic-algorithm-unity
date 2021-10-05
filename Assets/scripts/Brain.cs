using UnityEngine;


namespace DefaultNamespace
{
    /// <summary>
    /// The class stores genomes and allows operations on them
    /// </summary>
    public class Brain
    {
        private int _size = 200; //brain size
        public int Step = 0; //current step pointing to vector
        public Vector3[] Vectors; //hold vectors (genomes)

        /// <summary>
        /// Init values for new created objects
        /// </summary>
        public Brain()
        {
            Vectors = new Vector3[_size];
            GenerateRandomVectors();
        }

        /// <summary>
        /// Creates a random path for the player
        /// </summary>
        private void GenerateRandomVectors()
        {
            for (int i = 0; i < Vectors.Length; i++)
            {
                Vectors[i] = new Vector3(Random.Range(10, -10), 0, Random.Range(10, -10));
            }
        }

        /// <summary>
        /// Copies genomes and returns them as a new object
        /// </summary>
        /// <returns>new brain</returns>
        public Brain Clone()
        {
            Brain clone = new Brain();
            for (int i = 0; i < Vectors.Length; i++)
            {
                clone.Vectors[i] = Vectors[i];
            }

            return clone;
        }

        /// <summary>
        /// Changes the set number of vectors to new ones
        /// </summary>
        public void Mutate()
        {
            float rate = 0.02f; //% of mutation
            for (int i = 0; i < Vectors.Length; i++)
            {
                float random = Random.Range(0.0f, 1.0f);
                if (random < rate)
                {
                    Vectors[i] = new Vector3(Random.Range(10, -10), 0, Random.Range(10, -10));
                }
            }
        }
    }
}