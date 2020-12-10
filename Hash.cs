using System.Collections.Generic;
using System;

namespace Hashing
{
    class Hash
    {
        private int m;//hash table length(key)
        private int n;//number of values insterted
        private double lf; //loading Factor
        private int bDepth; //currently the deepest bucket's size
        private int bDLimit; // bucket depth limit
        public LinkedList<HashValue>[] HashTable;
        public Hash(int size, double loadingFactor, int bucketDepthLimit)
        {
            this.m = size;
            this.n = 0;
            this.lf = loadingFactor;
            this.bDLimit = bucketDepthLimit;
            this.bDepth = 0;
            this.HashTable = new LinkedList<HashValue>[size];
            for (int i = 0; i < size; i++)
            {
                this.HashTable[i] = new LinkedList<HashValue>();
            }
        }
        public int KeyLength
        {
            get
            {
                return this.m;
            }
        }
        public int DataLength
        {
            get
            {
                return this.n;
            }
            set
            {
                this.n = value;
            }
        }
        public double LoadingFactor
        {
            get
            {
                return this.lf;
            }
        }
        public int CurrentBucketDepth
        {
            get
            {
                return this.bDepth;
            }
            set
            {
                this.bDepth = value;
            }
        }
        public int BucketDepthLimit
        {
            get
            {
                return this.bDLimit;
            }
        }
        public void countBucketDepth()
        {
            int currDepth;
            int maxDepth = 0;
            for (int i = 0; i < this.KeyLength; i++)
            {
                currDepth = this.HashTable[i].Count;
                if (currDepth > maxDepth)
                    maxDepth = currDepth;
            }
            this.CurrentBucketDepth = maxDepth;
        }
        public void Remove(int value)
        {
            int hashKey = value % this.m;
            LinkedListNode<HashValue> node = this.HashTable[hashKey].First;
            if (node == null)
            {
                return;
            }
            else if (value < node.Value.Value)
            {
                return;
            }
            else
            {
                while (value > node.Value.Value)
                {
                    if (node.Next != null)
                    {
                        node = node.Next;
                    }
                    else
                    {
                        return;
                    }
                }
                if (value == node.Value.Value)
                {
                    node.Value.Occurrence--;
                    if (node.Value.Occurrence == 0)
                    {
                        this.HashTable[hashKey].Remove(node);
                        this.DataLength--;
                        this.countBucketDepth();
                    }
                }
                else
                {
                    return;
                }
            }
        }
        public void Add(int value, int occ)
        {
            this.DataLength++;
            int hashKey = Math.Abs(value) % this.m;
            HashValue hashVal = new HashValue(value, m);
            hashVal.Occurrence += occ;
            LinkedListNode<HashValue> node = this.HashTable[hashKey].First;
            if (node == null)
            {
                this.HashTable[hashKey].AddLast(hashVal);
            }
            else if (value < node.Value.Value)
            {
                this.HashTable[hashKey].AddFirst(hashVal);
            }
            else
            {
                while (value > node.Value.Value)
                {
                    if (node.Next != null)
                    {
                        node = node.Next;
                    }
                    else
                    {
                        this.HashTable[hashKey].AddAfter(node, hashVal);
                        this.countBucketDepth();
                        return;
                    }
                }
                if (value == node.Value.Value)
                {
                    node.Value.Occurrence++; //adding same value doesnt increase linked list length and data length of hashTable so complexity stays same
                    this.n--;
                }
                else
                {
                    this.HashTable[hashKey].AddBefore(node, hashVal);
                }
            }
            this.countBucketDepth();
        }

        private static int getNextKey(int lastKey)
        {
            bool isPrime;
            do
            {
                lastKey++;
                isPrime = true;
                for (int i = 2; i < lastKey; i++)
                {
                    if (lastKey % i == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
            } while (!isPrime);
            return lastKey;
        }
        public static void ReHash(ref Hash hash)
        {
            int lastPrime = hash.m;
            int newPrime = Hash.getNextKey(lastPrime);
            Hash newHash = new Hash(newPrime, hash.LoadingFactor, hash.BucketDepthLimit);
            foreach (LinkedList<HashValue> h in hash.HashTable)
            {
                foreach (HashValue v in h)
                {
                    newHash.Add(v.Value, v.Occurrence);
                }
            }
            newHash.countBucketDepth();
            hash = newHash;
        }
    }
    public class HashValue
    {
        private int intVal;  //unhashed value
        private int key; //hashed value
        private int occ; //occurrence
        public HashValue(int value, int key)
        {
            this.key = key;
            this.intVal = value;
            this.occ = 0;
        }
        public override string ToString()
        {
            return intVal.ToString();
        }
        public int Value
        {
            get
            {
                return intVal;
            }
        }
        public int Key
        {
            get
            {
                return key;
            }
        }
        public int Occurrence
        {
            get
            {
                return occ;
            }
            set
            {
                occ = value;
            }
        }
    }
}
