using Emgu.CV;

namespace ARExercise
{
    public static class Markers
    {
        private static List<Tuple<string, List<Tuple<int, Matrix<Byte>>>>>? allMarkers;
        public static List<Tuple<string, List<Tuple<int, Matrix<Byte>>>>> AllMarkers
        {
            get
            {
                if (allMarkers == null)
                {
                    var markers = new List<Tuple<string, List<Tuple<int, Matrix<Byte>>>>>();
                    markers.Add(new Tuple<string, List<Tuple<int, Matrix<byte>>>>("Marker1", GetAllRotations(Marker1)));
                    markers.Add(new Tuple<string, List<Tuple<int, Matrix<byte>>>>("Marker2", GetAllRotations(Marker2)));
                    markers.Add(new Tuple<string, List<Tuple<int, Matrix<byte>>>>("Marker3", GetAllRotations(Marker3)));
                    markers.Add(new Tuple<string, List<Tuple<int, Matrix<byte>>>>("Marker4", GetAllRotations(Marker4)));
                    markers.Add(new Tuple<string, List<Tuple<int, Matrix<byte>>>>("Marker5", GetAllRotations(Marker5)));
                    markers.Add(new Tuple<string, List<Tuple<int, Matrix<byte>>>>("Marker6", GetAllRotations(Marker6)));
                    markers.Add(new Tuple<string, List<Tuple<int, Matrix<byte>>>>("Marker7", GetAllRotations(Marker7)));
                    markers.Add(new Tuple<string, List<Tuple<int, Matrix<byte>>>>("Marker8", GetAllRotations(Marker8)));

                    allMarkers = markers;
                }
                return allMarkers;
            }
        }

        public static Matrix<byte> Marker1 = new(new byte[,]
        {
            {255,255,255,0  },
            {255,255,255,0  },
            {255,255,255,255},
            {255,255,255,255},
        });
        public static Matrix<byte> Marker2 = new(new byte[,]
        {
            {255,255,255,255},
            {255,255,255,0  },
            {255,255,255,255},
            {255,255,255,255},
        });
        public static Matrix<byte> Marker3 = new(new byte[,]
        {
            {255,255,255,255},
            {255,255,255,255},
            {255,255,0  ,0  },
            {255,255,255,255},
        });
        public static Matrix<byte> Marker4 = new(new byte[,]
        {
            {255,255,255,0  },
            {255,255,0  ,0  },
            {255,255,255,255},
            {255,255,255,255},
        });
        public static Matrix<byte> Marker5 = new(new byte[,]
        {
            {255,255,255,255},
            {255,255,255,255},
            {255,255,255,0  },
            {255,255,255,0  },
        });
        public static Matrix<byte> Marker6 = new(new byte[,]
        {
            {255,255,255,255},
            {255,255,0  ,0  },
            {255,255,255,255},
            {255,255,255,255},
        });
        public static Matrix<byte> Marker7 = new(new byte[,]
        {
            {255,255,255,255},
            {255,255,255,255},
            {255,255,0  ,0  },
            {255,255,255,0  },

        });
        public static Matrix<byte> Marker8 = new(new byte[,]
        {
            {255,255,255,255},
            {255,255,255,255},
            {255,255,255,0  },
            {255,255,255,255},
        });


        private static List<Tuple<int, Matrix<byte>>> GetAllRotations(Matrix<byte> original)
        {
            var rotations = new List<Tuple<int, Matrix<byte>>>();
            rotations.Add(new Tuple<int, Matrix<byte>>(0, original));

            var m = original.Data;
            int angle = 90;

            for (int i = 0; i < 3; i++)
            {
                Matrix<byte> rot = new(new byte[,]
               {
                    { m[0,3],m[1,3],m[2,3],m[3,3]},
                    { m[0,2],m[1,2],m[2,2],m[3,2]},
                    { m[0,1],m[1,1],m[2,1],m[3,1]},
                    { m[0,0],m[1,0],m[2,0],m[3,0]},
               });
                rotations.Add(new Tuple<int, Matrix<byte>>(angle, rot));
                m = rot.Data;
                angle += 90;
            }
            return rotations;
        }

        public static bool CompareMarkers(Matrix<byte> m1, Matrix<byte> m2)
        {
            if (m1.Size == m2.Size)
            {
                for (int x = 0; x < m1.Size.Width; x++)
                {
                    for (int y = 0; y < m1.Size.Height; y++)
                    {
                        if (m1.Data[x, y] != m2.Data[x, y])
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                Console.WriteLine("Not Same Size");
                return false;
            }
        }

        public static void ReadMarkers()
        {
            var markers = AllMarkers;
            int i = 1;

            foreach (var group in markers)
            {
                Console.WriteLine("------{0}------", group.Item1);
                foreach (var rotation in group.Item2)
                {
                    Console.WriteLine("Rotation: " + rotation.Item1);
                    WriteOutMarker(rotation.Item2);
                }
                i++;
            }
        }

        public static void WriteOutMarker(Matrix<Byte> marker)
        {
            for (int x = 0; x < marker.Size.Width; x++)
            {
                for (int y = 0; y < marker.Size.Height; y++)
                {
                    var value = marker.Data[x, y];
                    if (value == 0)
                        Console.ForegroundColor = ConsoleColor.Gray;
                    else if (value == 255)
                        Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("{0,-5}", value);
                }
                Console.Write("\n");
            }
            Console.ResetColor();
        }
    }
}
