using System;
using System.Data;

public class DataSource
{
    private int[,] Arr;

    int[,] arr = new int[,]
    {
            {1  ,-1     ,1000   ,2600   ,4400   },
            {2  ,-1     ,-1     ,-1     ,4200   },
            {3  ,-1     ,-1     ,-1     ,0      },
            {4  ,-1     ,-1     ,3300   , -1    },
            {5  ,1100   ,-1     ,-1     , -1    },
            {6  ,-1     ,-1     ,-1     , 5300  },
            {7  ,-1     ,-1     ,-1     , 0     },
            {8  ,-1     ,2000   , -1    , -1    },
            {9  ,-1     , -1    , 2700  , -1    },
            {10 ,-1     , -1    , -1    , -1    },
            {11 , -1    , -1    , -1    , 2800  },
            {12 , -1    , -1    , 3500  , -1    },
            {13 , -1    , -1    , -1    , -1    },
            {14 , -1    , -1    , -1    , -1    },
            {15 , -1    , -1    , -1    , 3800  },
            {16 , -1    , -1    , -1    , -1    },
            {17 , -1    , -1    , -1    , -1    },
            {18 , -1    , -1    , 2300  , -1    },
            {19 , -1    , 1000  , -1    , 3400  },
            {20 , -1    , -1    , -1    , -1    },
            {21 , 300   , -1    , -1    , -1    },
            {22 , -1    , -1    , -1    , -1    },
            {23 , -1    , -1    , 2800  , 4500  },
            {24 , -1    , -1    , -1    , -1    },
            {25 , -1    , -1    , -1    , -1    },
            {26 , 1700  , -1    , -1    , -1    },
            {27 , -1    , 1200  , -1    , 2850  },
            {28 , -1    , -1    , 2200  , -1    },
            {29 , -1    , 0     , -1    , 3700  },
            {30 , 400   , 500   , 2500  , 3650  },
            {31 , 300   , 400   , 2800  , 3300  }
    };
    int[,] arr1 = new int[,]
    {
            {1  ,-1     ,1000   ,2600   ,4400   },
            {2  ,500    ,-1     ,-1     ,4200   },
            {3  ,-1     ,-1     ,3300   , -1    }
    };

    int[,] arr3 = new int[,]
    {
            {1  ,-1     ,1000   ,2600   ,4400   },
            {2  ,-1     ,-1     ,-1     ,4200   },
            {3  ,-1     ,-1     ,-1     ,-1     },
            {4  ,-1     ,-1     ,3300   , -1    },
            {5  ,1100   ,-1     ,-1     , -1    },
            {6  ,-1     ,-1     ,-1     , 5300  },
            {7  ,-1     ,-1     ,-1     ,-1     },
            {8  ,-1     ,2000   , -1    , -1    },
            {9  ,-1     , -1    , 2700  , -1    },
            {10 ,-1     , -1    , -1    , -1    },
            {11 , -1    , -1    , -1    , 2800  },
            {12 , -1    , -1    , 3500  , -1    },
            {13 , -1    , -1    , -1    , -1    },
            {14 , -1    , -1    , -1    , -1    },
            {15 , -1    , -1    , -1    , 3800  },
            {16 , -1    , -1    ,-1     ,-1     }
    };


    public DataTable CreateDataTable()
    {
        Arr = arr3;

        DataTable table = new DataTable() { TableName = "YDJQTB" };

        table.Columns.Add("date", typeof(Int32));
        table.Columns.Add("TrafficCases", typeof(Int32));
        table.Columns.Add("ForHelpCases", typeof(Int32));
        table.Columns.Add("CriminalCases", typeof(Int32));
        table.Columns.Add("SecurityCases", typeof(Int32));

        int cols = Arr.Rank;
        for (int i = 0; i < Arr.GetLength(cols -2); i++)
        {
            DataRow row = table.NewRow();
            for (int j = 0; j < Arr.GetLength(cols - 1); j++)
            {
                row[j] = Arr[i,j];
            }
            table.Rows.Add(row);
        }

        return table;
    }
}