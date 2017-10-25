using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;

namespace RangePermute
{
    public class RangeEnumerable
    {
        struct AesRandomizer
        {
            ICryptoTransform encryptor;

            public AesRandomizer(byte[] key)
            {
                var aesAlg = new AesManaged
                {
                    KeySize = 128,
                    Key = key,
                    BlockSize = 128,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.None,
                    IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } // we want repeatability, so '0' nonce
                };

                encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            }

            public BigInteger Randomize(BigInteger input)
            {
                var byteArr = input.ToByteArray();
                var fullArr = new byte[16];
                bool signExtend = false;
                for (int i = 0; i < 16; i++)
                {
                    if (i < byteArr.Length)
                    {
                        fullArr[i] = byteArr[i];
                    }
                    if (i == byteArr.Length - 1)
                    {
                        if (byteArr[i] == 0xff)
                        {
                            signExtend = true;
                            continue;
                        }
                        break;
                    }

                    if (signExtend)
                    {
                        fullArr[i] = 0xff;
                    }
                }
                var encoded = encryptor.TransformFinalBlock(fullArr, 0, fullArr.Length);
                var encodedPosBuff = new byte[encoded.Length + 1];
                Array.Copy(encoded, encodedPosBuff, encoded.Length);
                var ret = new BigInteger(encodedPosBuff);
                return ret;
            }

        }

        struct IndexEncrypter
        {
            ulong _max;
            AesRandomizer[] _randomizers;
            BigInteger _a, _b;

            public IndexEncrypter(ulong max, AesRandomizer[] randomizers)
            {
                _max = max;
                _randomizers = randomizers;
                _a = _b = new BigInteger((ulong)(Math.Ceiling(Math.Sqrt(max))));
            }

            public ulong Encrypt(ulong index)
            {
                if (index > _max)
                {
                    throw new IndexOutOfRangeException();
                }

                var biIndex = new BigInteger(index);
                return FeistelEncrypt(biIndex);
            }

            ulong FeistelEncrypt(BigInteger biIndex)
            {
                do
                {
                    biIndex = _Feistel(biIndex);
                } while (biIndex >= _max);
                return (ulong)biIndex;
            }

            BigInteger _Feistel(BigInteger m)
            {
                var l = m % _a;
                var r = m / _a;

                for (int j = 0; j < _randomizers.Length; j++)
                {
                    BigInteger tmp;
                    if (j % 2 == 0)
                    {
                        tmp = (l + _randomizers[j].Randomize(r)) % _a;
                    }
                    else
                    {
                        tmp = (l + _randomizers[j].Randomize(r)) % _b;
                    }
                    l = r;
                    r = tmp;
                }

                if (_randomizers.Length % 2 == 1)
                {
                    return _a * l + r;
                }
                else
                {
                    return _a * r + l;
                }
            }
        }

        public static IEnumerable<ulong> Range(ulong max, int seed = 0)
        {

            Random rng = null;
            if (seed != 0)
            {
                rng = new Random(seed);
            }
            else
            {
                rng = new Random();
            }

            var numRandomizers = 3; // 3 is enough for a correct permutation
            var randomizers = new AesRandomizer[numRandomizers];

            using (var sha = new SHA512Managed())
            {
                var seedBuff = new byte[16 * numRandomizers];
                rng.NextBytes(seedBuff);
                var hash = sha.ComputeHash(seedBuff);

                for (int i = 0; i < numRandomizers; i++)
                {
                    var key = new byte[16];
                    Array.ConstrainedCopy(hash, i * 16, key, 0, 16);
                    randomizers[i] = new AesRandomizer(key);
                }
            }

            var ie = new IndexEncrypter(max, randomizers);
            for (ulong i = 0; i < max; i++)
            {
                var ret = ie.Encrypt(i);
                yield return ret;
            }

        }

    }
}
