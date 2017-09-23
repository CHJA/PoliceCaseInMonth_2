using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HighResolutionApps.Interfaces;
using VisualContorlBase;
using System.Data;
using Telerik.Windows.Controls.ChartView;
using Telerik.Charting;
using Telerik.Windows.Controls;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using HRVPoliceCaseInMonthControl_ZZGA;
using Telerik.Windows.Data;

namespace HighResolutionApps.VisualControls.PoliceCaseInMonth_2
{
    /// <summary>
    /// PoliceCaseInMonth_2.xaml 的交互逻辑
    /// </summary>
    public partial class PoliceCaseInMonth_2 : UserControl, IVisualControl, IPageEvent
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public PoliceCaseInMonth_2()
        {
            InitializeComponent();
            // 必要 样式使用
            HrvVisualControlStyles.StyleManager.GetResourceDictionnary(this, m_WindowStyleName);
            // 区分当前运行环境
            if (CurrentRunderUnitEnvironment.Mode == EnvironmentMode.Designer)
            {
                // 运行环境是设计端
                // 界面渲染  在设计端调用  在其他运行环境无需调用
                LoadContent();
            }

        }

        // ===================================================================================================================
        // 共有四类警情：
        // 当月刑事，CriminalCases，黄色曲线
        // 当月交通，SecurityCases，蓝色曲线
        // 去年刑事，TrafficCases，红色曲线
        // 去年交通，ForHelpCases，绿色曲线


        /// <summary>
        /// 创建自定义数据，填充到dataTable
        /// </summary>
        /// <param name="dataTable"></param>        
        public void InitInputDataTable()
        {
            SystemHelper.logger.LogDebug("调试信息=====> InitInputDataTable 版本：2017年9月11日修改<=====");
            try
            {
                DataTable dataTable = new DataTable { TableName = "YDJQTB" };

                dataTable.Columns.Add("date", typeof(Int32));
                dataTable.Columns.Add("criminal", typeof(Int32));
                dataTable.Columns.Add("traffic", typeof(Int32));
                dataTable.Columns.Add("crim_lastyear", typeof(Int32));
                dataTable.Columns.Add("traf_lastyear", typeof(Int32));

                dataTable.Rows.Add(1, -1, 1000, 2600, 4400);
                dataTable.Rows.Add(2, -1, -1, -1, 4200);
                dataTable.Rows.Add(4, -1, -1, 3300, -1);
                dataTable.Rows.Add(5, 1100, -1, -1, -1);
                dataTable.Rows.Add(6, -1, -1, -1, 5300);
                dataTable.Rows.Add(8, -1, 2000, -1, -1);
                dataTable.Rows.Add(9, -1, -1, 2700, -1);
                dataTable.Rows.Add(11, -1, -1, -1, 2800);
                dataTable.Rows.Add(12, -1, -1, 3500, -1);
                dataTable.Rows.Add(13, -1, -1, -1, -1);
                dataTable.Rows.Add(15, -1, -1, -1, 3800);
                dataTable.Rows.Add(18, -1, -1, 2300, -1);
                dataTable.Rows.Add(19, -1, 1000, -1, 3400);
                dataTable.Rows.Add(21, 300, -1, -1, -1);
                dataTable.Rows.Add(23, -1, -1, 2800, 4500);
                dataTable.Rows.Add(25, -1, -1, -1, -1);
                dataTable.Rows.Add(26, 1700, -1, -1, -1);
                dataTable.Rows.Add(27, -1, 1200, -1, 2850);
                dataTable.Rows.Add(28, -1, -1, 2200, -1);
                dataTable.Rows.Add(29, -1, 0, -1, 3700);
                dataTable.Rows.Add(30, 400, 500, 2500, 3650);
                dataTable.Rows.Add(31, 300, 400, 2800, 3300);

                m_InputDataTable = dataTable;
            }
            catch (Exception ex)
            {
                SystemHelper.logger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString(), ex.ToString());
            }

        }

        /// <summary>
        /// 加载数据到图形界面
        /// </summary>
        void InputDataTable2Chart(DataTable dataTable)
        {
            SystemHelper.logger.LogDebug("调试信息=====> InputDataTable2Chart 版本：2017年9月11日修改<=====");
            if ((dataTable == null) || (dataTable.Rows.Count == 0))
            {
                //SystemHelper.logger.LogDebug("调试信息=====> dataTable.Rows.Count=" + dataTable.Rows.Count);
                SystemHelper.logger.LogDebug("调试信息=====> InputDataTable2Chart 没有数据-DataTable为空 <=====");
                return;
            }

            try
            {
                // ...
                int yMax = 200; // y轴最大刻度
                int yMin = 0;   // y轴最小刻度
                int yMajorStep = 0;    // y轴刻度间隔

                // 创建提取数据的时间点
                DateTime timePoint = DateTime.Now;//= new DateTime(2017, 9, 5);
                SystemHelper.logger.LogDebug("调试信息=====> " + "今日时间=" + timePoint);
                int timeSpan = 31;
                DataTable dt = new DataTable();
                dt = ExtractData(dataTable, timePoint, timeSpan); // 取30天的数据

                // 第一部分：整理数据   
                dt = CheckData(dt);

                // 第二部分：规划chart框架，包括坐标轴最大最小值
                yMax = (int)(GetMaxY(dt) * 1.3);
                //yMajorStep = (int)(yMax / 5) + 1;

                // 设置坐标轴
                VertiAxis.Maximum = yMax;
                VertiAxis.Minimum = yMin;
                //VertiAxis.MajorStep = yMajorStep;

                // 第三部分：加载数据
                policeCasesChart.Series.Clear();
                InitXAxis(timePoint, timeSpan);   // 初始化X轴

                SplineAreaSeries splineAreaSeries = null; // 散点曲线区域

                List<CaseInMonth> clist = new List<CaseInMonth>();
                clist = DrillTable(dt);
                //SystemHelper.logger.LogDebug("调试信息=====> " + "clist保存的对象个数= " + clist.Count);
                // 按照cm均值 从大到小输出
                for (int i = clist.Count; i > 0; i--)
                {
                    //SystemHelper.logger.LogDebug("调试信息=====> " + "clist=>>对象.序号= " + clist[i - 1].CaseOrder);
                    splineAreaSeries = new SplineAreaSeries();
                    foreach (KeyValuePair<int, int> kvp in clist[i - 1].CaseSortedList)
                    {
                        CategoricalDataPoint point = new CategoricalDataPoint()
                        {
                            Category = kvp.Key.ToString(),
                            Value = kvp.Value
                        };
                        //SystemHelper.logger.LogDebug("调试信息=====> " + "键-值对==(" + kvp.Key + "," + kvp.Value + ")");
                        splineAreaSeries.DataPoints.Add(point);
                    }

                    // 将 散点曲线区域 对象添加到 Chart中
                    policeCasesChart.Series.Add(splineAreaSeries);
                    // 绘制填充效果
                    FillSeriesBackground(splineAreaSeries, clist[i - 1].CaseOrder);
                    //SystemHelper.logger.LogDebug("调试信息=====> " + "加载clist中对象的序号==>" + i);
                }

                // 添加显示副坐标轴的最后一个series
                SplineAreaSeries endSeries = new SplineAreaSeries
                {
                    HorizontalAxis = new DateTimeCategoricalAxis()
                    {
                        VerticalLocation = AxisVerticalLocation.Top,

                        LineStroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x08, 0x9C, 0xFF)), // 轴线的颜色
                        LineThickness = 3,      // 轴线的粗细
                        MajorTickLength = 0,    // 坐标线长度
                        ShowLabels = false,     // 不显示坐标值
                        IsEnabled = false
                    },
                    VerticalAxis = new LinearAxis()
                    {
                        HorizontalLocation = AxisHorizontalLocation.Right,

                        LineStroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x08, 0x9C, 0xFF)), // 轴线的颜色
                        LineThickness = 3,   // 轴线的粗细
                        MajorTickLength = 0, // 坐标线长度
                        ShowLabels = false,  // 不显示坐标值
                        IsEnabled = false
                    }
                };
                policeCasesChart.Series.Add(endSeries);

                SystemHelper.logger.LogDebug("调试信息=====> " + "图形加载完毕!");
            }
            catch (Exception ex)
            {
                SystemHelper.logger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString(), ex.ToString());
            }

        }

        /// <summary>
        /// 初始化X轴
        /// </summary>
        /// <param name="timePoint"></param>
        private void InitXAxis(DateTime timePoint, int span)
        {
            DateTime endDate = timePoint;
            TimeSpan timeSpan = TimeSpan.FromDays(span); 
            DateTime startDate = endDate - timeSpan;
            string timeFormat = "Mdd";
            SplineAreaSeries series = new SplineAreaSeries();

            for (int i = 0; i < span; i++)
            {
                series.DataPoints.Add(new CategoricalDataPoint() { Category = startDate.AddDays(i).ToString(timeFormat) });
            }

            policeCasesChart.Series.Add(series);
        }

        /// <summary>
        /// 提取数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private DataTable ExtractData(DataTable dt, DateTime timePoint, int span)
        {
            SystemHelper.logger.LogDebug("调试信息=====> " + "ExtractData <====");

            DataTable table = new DataTable();
            //DateTime latest = new DateTime(2017, 8, 31);
            DateTime endDate = timePoint;
            TimeSpan timeSpan = TimeSpan.FromDays(span);
            DateTime startDate = endDate - timeSpan;
            string timeFormat = "Mdd";

            try
            {
                string startDateStr = startDate.ToString(timeFormat);
                string endDateStr = endDate.ToString(timeFormat);

                table = dt.Clone();

                dt.Columns[0].ColumnName = "date";
                string expr = "date>=" + startDateStr + " AND date<" + endDateStr;
                string order = "date ASC";
                DataRow[] foundRows = dt.Select(expr, order);
                foreach (var r in foundRows)
                {
                    SystemHelper.logger.LogDebug("调试信息=====> " + "ExtractData <===row-->" + r);

                    table.ImportRow(r);
                }
            }
            catch (Exception ex)
            {
                SystemHelper.logger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString(), ex.ToString());
            }

            return table;
        }

        /// <summary>
        /// 检查数据
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private DataTable CheckData(DataTable dataTable)
        {
            SystemHelper.logger.LogDebug("调试信息=====> CheckData " + "检查DataTable <=====");

            if (dataTable == null)
            {
                SystemHelper.logger.LogDebug("调试信息=====> " + "DataTable is Null！");
                return null;
            }

            if (dataTable.Rows.Count == 0)
            {
                SystemHelper.logger.LogDebug("调试信息=====> " + "没有数据！");
                return null;
            }

            int cols = dataTable.Columns.Count;

            if (cols == 1)
            {
                SystemHelper.logger.LogDebug("调试信息=====> " + "只有一列数据！");
                return null;
            }

            if (cols < 5)
            {
                SystemHelper.logger.LogDebug("调试信息=====> " + "DataTable仅有" + cols + "列数据！，需要5列数据");
            }

            try
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        if ((dataTable.Rows[i][j] == DBNull.Value) || (Convert.ToInt32(dataTable.Rows[i][j]) == -1))
                        {
                            SystemHelper.logger.LogDebug("调试信息=====> " + "空值！");
                        }

                        if (Convert.ToInt32(dataTable.Rows[i][j]) < -1)
                        {
                            dataTable.Rows[i][j] = 0;
                            SystemHelper.logger.LogDebug("调试信息=====> " + "负值！");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                SystemHelper.logger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString(), ex.ToString());
            }

            SystemHelper.logger.LogDebug("调试信息=====> CheckData " + "检查DataTable完成！<=====");
            return dataTable;
        }

        private int GetMaxY(DataTable dt)
        {
            int max = 0;
            List<int> caseList = new List<int>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 1; j < dt.Columns.Count; j++)
                {
                    // DBNull.Value表示数据库表中不存在的值
                    if ((dt.Rows[i][j] != DBNull.Value) && (Convert.ToInt32(dt.Rows[i][j]) >= 0))
                    {
                        caseList.Add(Convert.ToInt32(dt.Rows[i][j]));
                    }
                }
            }
            max = caseList.Max();

            return max;
        }

        /// <summary>
        /// DrillTable
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private List<CaseInMonth> DrillTable(DataTable dataTable)
        {
            SystemHelper.logger.LogDebug("调试信息=====> <=====");

            List<CaseInMonth> list = new List<CaseInMonth>();

            try
            {
                if (dataTable == null)
                {
                    SystemHelper.logger.LogDebug("调试信息=====> " + "空表！");
                    return null;
                }

                if (dataTable.Rows.Count == 0)
                {
                    SystemHelper.logger.LogDebug("调试信息=====> " + "没有数据！");
                    return null;
                }

                int cols = dataTable.Columns.Count;

                for (int j = 1; j < cols; j++)
                {
                    SortedList<int, int> sortedList = new SortedList<int, int>();
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        if ((dataTable.Rows[i][j] != DBNull.Value) && (Convert.ToInt32(dataTable.Rows[i][j]) != -1))
                        {
                            try
                            {
                                sortedList.Add(Convert.ToInt32(dataTable.Rows[i][0]), Convert.ToInt32(dataTable.Rows[i][j]));

                            }
                            catch (ArgumentException)
                            {
                                SystemHelper.logger.LogDebug("调试信息=====> 数据表 第" + (i + 1)
                                    + "行第" + 1 + "列的数据" + dataTable.Rows[i][0].ToString() + "已经存在，日期重复！");
                                SystemHelper.logger.LogDebug("调试信息=====> 跳过本行数据！");
                                continue; // 跳过本行数据
                            }
                        }
                    }
                    sortedList.TrimExcess();

                    CaseInMonth cm = new CaseInMonth(j, sortedList);
                    list.Add(cm);
                    // 将cm对象排序
                    list.Sort((x, y) =>
                    {
                        return (x.Average.CompareTo(y.Average) != 0) ? x.Average.CompareTo(y.Average) : x.CaseOrder.CompareTo(y.CaseOrder);
                    });

                }
                list.TrimExcess();
                //SystemHelper.logger.LogDebug("调试信息=====> "
                //    + "链表List保存的CaseInMonth对象的个数为：" + list.Count + ",应该为 " + (cols - 1) + " 个对象");

            }
            catch (Exception ex)
            {
                SystemHelper.logger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString(), ex.ToString());
            }

            return list;
        }

        /// <summary>
        /// 绘制图形填充效果
        /// </summary>
        /// <param name="series"></param>
        /// <param name="caseSort"></param>
        private static void FillSeriesBackground(SplineAreaSeries series, int caseSort)
        {
            try
            {
                // 画刷
                LinearGradientBrush brush = new LinearGradientBrush
                {
                    // 垂直渐变
                    EndPoint = new Point(0.5, 1),
                    StartPoint = new Point(0.5, 0)
                };

                series.StrokeThickness = 2; // 设置 线 的粗细为2

                switch (caseSort)
                {
                    case 1:
                        series.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xDE, 0xFF, 0x65));
                        // 设置区域渐变颜色：黄色渐变，刑事警情，1
                        brush.GradientStops.Add(new GradientStop(color: Color.FromArgb(0xCC, 0xDE, 0xFF, 0x65), offset: 0));
                        brush.GradientStops.Add(new GradientStop(color: Color.FromArgb(0x00, 0x1A, 0x3D, 0x47), offset: 1));
                        break;
                    case 2:
                        series.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x2F, 0x9C, 0xFF));
                        // 设置区域渐变颜色：蓝色渐变，治安警情，2
                        brush.GradientStops.Add(new GradientStop(color: Color.FromArgb(0xFF, 0x2F, 0x9C, 0xFF), offset: 0));
                        brush.GradientStops.Add(new GradientStop(color: Color.FromArgb(0x00, 0x1A, 0x3D, 0x47), offset: 1));
                        brush.GradientStops.Add(new GradientStop(color: Color.FromArgb(0xCC, 0x2E, 0x98, 0xF8), offset: 0.05));
                        break;
                    case 3:
                        series.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xE8, 0x0B, 0x0B));    // 设置 线 的颜色
                        // 设置区域渐变颜色：红色渐变，交通事故，3
                        brush.GradientStops.Add(new GradientStop(color: Color.FromArgb(0xFF, 0xE8, 0x0B, 0x0B), offset: 0));
                        brush.GradientStops.Add(new GradientStop(color: Color.FromArgb(0x00, 0xEA, 0x3D, 0x47), offset: 1));
                        brush.GradientStops.Add(new GradientStop(color: Color.FromArgb(0xCC, 0xFF, 0x30, 0x54), offset: 0.05));
                        break;
                    case 4:
                        series.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x4D, 0xFF, 0xAE));
                        // 设置区域渐变颜色：绿色渐变，群众求助，4
                        brush.GradientStops.Add(new GradientStop(color: Color.FromArgb(0xCC, 0x4D, 0xFF, 0xAE), offset: 0));
                        brush.GradientStops.Add(new GradientStop(color: Color.FromArgb(0x00, 0x1A, 0x3D, 0x47), offset: 1));
                        break;

                }

                series.Fill = brush;    // 填充
            }
            catch (Exception ex)
            {
                SystemHelper.logger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString(), ex.ToString());
            }

        }


        #region  所需属性

        #region 1.置标题字体样式
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TitleFontFamilyProperty = DependencyProperty.Register("TitleFontFamily", typeof(Enum_MyFontFamily), 
            typeof(PoliceCaseInMonth_2), new PropertyMetadata(Enum_MyFontFamily.宋体, OnTitleFontFamilyChanged));
        /// <summary>
        /// 
        /// </summary>
        public Enum_MyFontFamily TitleFontFamily
        {
            set { this.SetValue(TitleFontFamilyProperty, value); }
            get { return (Enum_MyFontFamily)this.GetValue(TitleFontFamilyProperty); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="e"></param>
        public static void OnTitleFontFamilyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            var control = (dp as PoliceCaseInMonth_2);
            control.ReRender(1);
            System.Drawing.Text.InstalledFontCollection font = new System.Drawing.Text.InstalledFontCollection();
            System.Drawing.FontFamily[] array = font.Families;
        }
        #endregion

        #region 2.设置标题字体颜色
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TitleForegroundColorProperty = DependencyProperty.Register("TitleForegroundColor", 
            typeof(Brush), typeof(PoliceCaseInMonth_2), new PropertyMetadata(Brushes.Black, OnTitleForegroundColorChanged));
        /// <summary>
        /// 
        /// </summary>
        public Brush TitleForegroundColor
        {
            set { this.SetValue(TitleForegroundColorProperty, value); }
            get { return (Brush)this.GetValue(TitleForegroundColorProperty); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="e"></param>
        public static void OnTitleForegroundColorChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            var control = (dp as PoliceCaseInMonth_2);
            control.ReRender(2);
        }

        #endregion

        #region 3.设置标题内容
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TextTitleProperty = DependencyProperty.Register("TextTitle", typeof(string), 
            typeof(PoliceCaseInMonth_2), new PropertyMetadata("月度警情同比走势", OnTextTitleChanged));
        /// <summary>
        /// 
        /// </summary>
        public string TextTitle
        {
            set { this.SetValue(TextTitleProperty, value); }
            get { return (string)this.GetValue(TextTitleProperty); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="e"></param>
        public static void OnTextTitleChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            var control = (dp as PoliceCaseInMonth_2);
            control.ReRender(3);
        }

        #endregion

        #region 4.标题字体大小
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TitleSizeProperty = DependencyProperty.Register("TitleSize", typeof(double), 
            typeof(PoliceCaseInMonth_2), new PropertyMetadata(30d, OnTitleSizeChanged));
        /// <summary>
        /// 
        /// </summary>
        public double TitleSize
        {
            set { this.SetValue(TitleSizeProperty, value); }
            get { return (double)this.GetValue(TitleSizeProperty); }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="e"></param>
        public static void OnTitleSizeChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            var control = (dp as PoliceCaseInMonth_2);
            control.ReRender(4);
        }

        #endregion

        /// <summary>
        /// 字体
        /// </summary>
        public enum Enum_MyFontFamily
        {
            ///
            微软雅黑,
            ///
            宋体,
            ///
            黑体,
            ///
            楷体
        };

        /// <summary>
        /// 属性返回值
        /// </summary>
        /// <param name="x"></param>
        void ReRender(int x)
        {
            switch (x)
            {
                case 1:
                    tbTitle.FontFamily = new FontFamily(TitleFontFamily.ToString());
                    break;
                case 2:
                    tbTitle.Foreground = TitleForegroundColor;
                    break;

                case 3:
                    tbTitle.Text = TextTitle;
                    break;
                case 4:
                    tbTitle.FontSize = TitleSize;
                    break;
            }
        }

        #endregion


        #region 依赖属性

        /// <summary>
        /// 注册属性，在控件序列化之前使用会发生序列化异常
        /// </summary>
        public DataTable InputDataTable
        {
            get { return (DataTable)GetValue(InputDataTableProperty); }
            set { SetValue(InputDataTableProperty, value); }
        }
        /// <summary>
        /// 私有属性的 DataTable 对象
        /// </summary>
        private DataTable m_InputDataTable = new DataTable();

        /// <summary>
        /// datatable
        /// </summary>
        public static readonly DependencyProperty InputDataTableProperty = DependencyProperty.Register(
            "InputDataTable", typeof(DataTable), typeof(PoliceCaseInMonth_2), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnInputDataTableChange)));


        /// <summary>
        /// 改变输入的datatable
        /// </summary> 
        private static void OnInputDataTableChange(DependencyObject o, DependencyPropertyChangedEventArgs ea)
        {
            try
            {
                //绑定数据
                PoliceCaseInMonth_2 gridControl = o as PoliceCaseInMonth_2;
                gridControl.ReRenderData();

            }
            catch (Exception ex)
            {
                SystemHelper.logger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString(), ex.ToString());
            }

        }
        /// <summary>
        /// 重绘数据图形界面
        /// </summary>
        void ReRenderData()
        {
            try
            {
                //调用绘制画面方法
                if (InputDataTable != null)
                {
                    SystemHelper.logger.LogDebug("调试信息=====> ReRenderData <=====");
                    InputDataTable2Chart(InputDataTable);
                }
                else
                {
                    SystemHelper.logger.LogDebug("调试信息=====> " + "InputDataTable未绑定数据！");
                }

            }
            catch (Exception ex)
            {
                SystemHelper.logger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString(), ex.ToString());
            }
        }

        #endregion 


        #region 标题修改 序列化控制
        /// <summary>
        /// 主标题
        /// </summary>
        private System.Windows.Controls.Label m_lb_ControlInstanceMainTitle;
        /// <summary>
        /// 副标题
        /// </summary>
        private System.Windows.Controls.Label m_lb_ControlInstanceSubTitle;
        /// <summary>
        /// 修改组件框中的标题
        /// </summary>
        public override void OnApplyTemplate()
        {
            try
            {
                base.OnApplyTemplate();
                m_lb_ControlInstanceMainTitle = GetTemplateChild("lb_ControlInstanceMainTitle") as System.Windows.Controls.Label;
                if (m_lb_ControlInstanceMainTitle != null)
                {
                    m_lb_ControlInstanceMainTitle.Content = m_ControlInstanceMainTitle;
                }
                m_lb_ControlInstanceSubTitle = GetTemplateChild("lb_ControlInstanceSubTitle") as System.Windows.Controls.Label;
                if (m_lb_ControlInstanceSubTitle != null)
                {
                    m_lb_ControlInstanceSubTitle.Content = m_ControlInstanceSubTitle;
                }
            }
            catch (Exception ex)
            {
                SystemHelper.logger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
              System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString(), ex.ToString());
            }

        }


        /// <summary>
        /// 重要 是否序列化内容  
        /// </summary>
        /// <returns></returns>
        public override bool ShouldSerializeContent()
        {
            return false;
        }
        #endregion


        #region 接口IVisualControl成员
        /// <summary>
        /// 组件背景窗口名称
        /// </summary>
        private string m_WindowStyleName;
        /// <summary>
        /// 组件背景窗口名称
        /// </summary>
        public string WindowStyleName
        {
            get { return m_WindowStyleName; }
            set { m_WindowStyleName = value; HrvVisualControlStyles.StyleManager.SetStyle(this, m_WindowStyleName); }
        }

        /// <summary>
        /// 加载组件中的显示内容 加载成功 isContentLoaded = true;
        /// </summary>
        /// <returns>加载完成并无错误返回true,有错误返回false</returns>
        public bool LoadContent()
        {
            try
            {
                if (CurrentRunderUnitEnvironment.Mode == EnvironmentMode.Designer)
                {
                    SystemHelper.logger.LogDebug("调试信息=====> HRV1PoliceCaseInMonthControl_ZZGA 版本：2017年9月11日修改<=====");
                    // 初始化数据
                    DataSource data = new DataSource();
                    m_InputDataTable = data.CreateDataTable();
                    //InitInputDataTable(m_InputDataTable);
                    // 绘制自定义数据
                    InputDataTable2Chart(m_InputDataTable);
                }
                GC.Collect();   // 强制内存回收
                // 渲染成功后
                isContentLoaded = true;
                return true;

            }
            catch (Exception ex)
            {
                SystemHelper.logger.LogError(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                              System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message.ToString(), ex.ToString());
                isContentLoaded = false;
                return false;
            }
        }


        /// <summary>
        /// 标识此组件的内容是否已加载，即LoadContent()方法已成功调用
        /// </summary>
        private bool m_isContentLoaded = false;
        /// <summary>
        /// 是否加载完成
        /// </summary>
        public bool isContentLoaded
        {
            get { return m_isContentLoaded; }
            private set { m_isContentLoaded = value; }
        }


        /// <summary>
        /// 加载顺序标识，数字越大越后加载，建议默认值为0，
        /// *此属性应注册为可在设计端修改的属性
        /// </summary>
        private int m_renderOrder = 0;
        /// <summary>
        /// 加载顺序
        /// </summary>
        public int RenderOrder
        {
            get { return m_renderOrder; }
            set { m_renderOrder = value; }
        }


        /// <summary>
        /// 本可视化组件类型的唯一标识
        /// 需要修改 guid
        /// </summary>
        public string ControlGUID
        {
            get { return "A9E4BD9A-C019-460D-8962-ED81CD8DF51A"; }
        }


        /// <summary>
        /// 本可视化组件类型名称
        /// 需要修改 组件名称
        /// </summary>
        public string ControlName
        {
            get { return "1.月度警情同比走势"; }
        }


        /// <summary>
        /// 实例名称即窗口默认主标题
        /// *此属性应注册为可在设计端修改的属性
        /// 此属性默认值建议与ControlName相同
        /// 在设计过程中允许设计人员给此实例命名，便于人工查找区分
        /// 在可视化组件选取列表中将展示此名称
        /// 如：组件名称为：表格组件，实例名称为：2015年度北京市人口统计表
        /// 提示：组件名称可由组件开发者动态生成，如上个例子中，可将dataTable/dataView的名称作为此项为空时的默认名称
        /// 需要修改 默认主标题名称
        /// </summary>
        private string m_ControlInstanceMainTitle = "默认主标题名称";
        /// <summary>
        /// 窗口默认主标题
        /// </summary>
        public string ControlInstanceMainTitle
        {
            get { return m_ControlInstanceMainTitle; }
            set
            {
                m_ControlInstanceMainTitle = value;
                if (m_lb_ControlInstanceMainTitle != null)
                {
                    m_lb_ControlInstanceMainTitle.Content = m_ControlInstanceMainTitle;
                }
            }
        }
        /// <summary>
        /// 实例名称即窗口默认副标题
        /// *此属性应注册为可在设计端修改的属性
        /// 在设计过程中允许设计人员给此实例命名，便于人工查找区分
        /// 在可视化组件选取列表中将展示此名称
        /// 如：组件名称为：表格组件，实例名称为：2015年度北京市人口统计表
        /// 提示：组件名称可由组件开发者动态生成，如上个例子中，可将dataTable/dataView的名称作为此项为空时的默认名称
        /// 需要修改 默认副标题名称
        /// </summary>
        private string m_ControlInstanceSubTitle = "默认副标题名称";
        /// <summary>
        /// 窗口默认副标题
        /// </summary>
        public string ControlInstanceSubTitle
        {
            get { return m_ControlInstanceSubTitle; }
            set
            {
                m_ControlInstanceSubTitle = value;
                if (m_lb_ControlInstanceSubTitle != null)
                {
                    m_lb_ControlInstanceSubTitle.Content = m_ControlInstanceSubTitle;
                }
            }
        }

        /// <summary>
        /// 组件实例描述信息
        /// 用于设计人员描述组件所呈现的相关业务信息
        /// 通过此信息，交互控制人员可分辨组件内显示的业务内容
        /// 需要修改 组件示例
        /// </summary>
        private string m_ControlInstanceDes = "组件示例";
        /// <summary>
        /// 组件描述信息
        /// </summary>
        public string ControlInstanceDes
        {
            get { return m_ControlInstanceDes; }
            set { m_ControlInstanceDes = value; }
        }



        /// <summary>
        /// 可视化组件的描述信息
        /// 包括呈现说明，应用描述
        /// 需要修改  组件描述
        /// </summary>
        public string ControlDescription
        {
            get { return "组件描述"; }
        }


        /// <summary>
        /// 可视化组件配置应用说明
        /// 此属性在页面设计人员引用此组件时提示时使用，告诉设计人员如何配置数据源之用
        /// 必须配置哪几项，属性间相互关系说明等
        /// 需要修改  配置说明
        /// </summary>
        public string ControlUseDescription
        {
            get { return "配置说明"; }
        }

        /// <summary>
        /// 可视化组件版本信息
        /// 需要修改  组件版本
        /// </summary>
        public Version Version { get { return new Version("1.0.0.1"); } }


        /// <summary>
        /// 依赖的可视化开发、运行环境版本号
        /// 开发时使用的开发、运行环境版本号        
        /// </summary>
        public Version HRVRuntimeVersion
        {
            get { return new Version("1.1.0.0"); }
        }


        /// <summary>
        /// 可视化组件所有权公司
        /// </summary>
        public string CompanyName
        {
            get { return "北京威创视讯信息系统有限公司"; }
        }



        /// <summary>
        /// 依赖动态库列表属性（高分平台dll,.net framework系统dll不用描述）
        /// 要求格式：版本号=动态库名称
        /// 样例    ：1.0.0.1=AForge.Imaging.Formats.dll
        /// </summary>
        public string[] DependentDllList
        {
            get
            {
                return new string[] { "" };
            }
        }


        /// <summary>
        /// 依赖文件资源列表属性
        /// 要求资源放置位置为启动程序(可视化组件dll)目录为起点的
        /// ../Files_Resource/xxxx.dll/文件夹内，xxxx为本可视化组件所在的dll名称
        /// 资源文件夹内可创建子文件夹
        /// </summary>
        public string[] DependentResourceFileList
        {
            get { return null; }
        }

        /// <summary>
        /// 依赖字体名称列表
        /// </summary>
        public string[] DependentFontList
        {
            get
            {
                string[] str = new string[1];
                str[0] = "宋体";
                return str;
            }
        }


        /// <summary>
        /// 获取复合数据源样例数据
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns></returns>
        public System.Data.DataTable getDataTableSample(string propertyName)
        {
            return null;
        }


        /// <summary>
        /// 通用控制触发窗口调用方法
        /// 通过此方法，可给最终用户提供对本组件的细节控制交互界面 预留
        /// 注意：如果需要与大屏幕上的渲染形成互动，则配置窗口改变的属性应都为同步属性
        /// </summary>
        public void ShowConfigurationWindow()
        {

        }

        /// <summary>
        /// 设置网络授权的ip地址
        /// </summary>
        /// <param name="ip">网络授权的IP地址</param>
        bool IVisualControl.setNetLicenseIP(string ip)
        {
            return false;
        }

        /// <summary>
        /// 独立线程、进程加载及运行标识属性
        ///  *此属性应注册为可在设计端修改的属性
        /// 一般组件请设置InMainProcess为默认值
        /// StandaloneThread：独立线程运行。StandaloneProcess：独立进程运行
        /// </summary>
        EnumRenderMethodType m_RenderMethod = EnumRenderMethodType.InMainProcess;
        /// <summary>
        /// 渲染模式
        /// </summary>
        public EnumRenderMethodType RenderMethod
        {
            get
            {
                return m_RenderMethod;
            }
            set
            {
                m_RenderMethod = value;
            }
        }

        /// <summary>
        /// 内存要求
        /// </summary>
        int m_MemoryRequest = 0;
        /// <summary>
        /// 内存需求
        /// </summary>
        public int MemoryRequest
        {
            get
            {
                return m_MemoryRequest;
            }
            set
            {
                m_MemoryRequest = value;
            }
        }

        /// <summary>
        /// 标识此可视化组件的实例化限制方式
        /// 大型可视化组件，如：3D，GIS等作为所有页面的公用底图使用的组件，可考虑多页面间共用一个实例。设置为：OnlyOneInstance
        /// 普通组件可创建多个实例。设置为：MultiInstance
        /// </summary>
        public EnumInstanceType InstanceType
        {
            get { return EnumInstanceType.MultiInstance; }
        }

        /// <summary>
        /// 标识此可视化组件可支持的windows操作系统版本情况
        /// 系统默认按照x86模式进行渲染，如果组件仅支持x64操作系统，则必须在设计时标识使用独立进程渲染
        /// 仅当此可视化组件被设置为独立进程渲染，同时此项标识为x64的情况下
        /// 渲染引擎可考虑采用x64模式渲染，以增加内存的支持和x64特性的保障。
        /// </summary>
        public EnumOperateSystemSupportType OperateSystemSupportType
        {
            get { return EnumOperateSystemSupportType.X86andX64; }
        }

        /// <summary>
        /// 可视化组件释放资源接口
        /// </summary>
        public void DisposeVisualControl()
        {

        }

        ///此组件在显示时是否可以任意移动、改变大小、角度改变
        ///避免有些组件被错误的移动到其他地方，比如标题，底图，背景图片等
        /// *此属性应注册为可在设计端修改的属性
        /// 默认为可移动,true
        private bool m_IsEditableInOperater = true;
        /// <summary>
        /// 是否可编辑
        /// </summary>
        public bool IsEditableInOperater
        {
            get { return m_IsEditableInOperater; }
            set { m_IsEditableInOperater = value; }
        }

        /// <summary>
        /// 是否可以跨屏，跨渲染节点显示
        ///  *此属性应注册为可在设计端修改的属性
        /// 默认为false
        /// 此属性为处理那些高分化改造不充分或无法改造的组件而设计
        /// </summary>
        private bool m_IsMustInOnlyOneMachine = false;
        /// <summary>
        /// 是否可以跨屏
        /// </summary>
        public bool IsMustInOnlyOneMachine
        {
            get { return m_IsMustInOnlyOneMachine; }
            private set { m_IsMustInOnlyOneMachine = value; }
        }

        #endregion


        #region IPageEvent接口
        /// <summary>
        /// 整个页面隐藏之后的通知方法
        /// </summary>
        public void AfterPageHide()
        {

        }
        /// <summary>
        /// 整个页面初始化之后的通知方法，区分于组件loaded，
        /// 此方法为所有组件均loaded完成之后触发
        /// </summary>
        public void AfterPageInit()
        {

        }
        /// <summary>
        /// 整个页面显示到大屏之后的通知
        /// </summary>
        public void AfterPageShow()
        {

        }

        /// <summary>
        /// 整个页面隐藏显示之前的通知方法
        /// </summary>
        public void BeforePageHide()
        {

        }

        /// <summary>
        /// 整个页面显示之前的通知方法
        /// </summary>
        public void BeforePageShow()
        {

        }

        /// <summary>
        /// 整个页面释放之前的通知方法，要求组件停止timer，处理逻辑，保存状态等
        /// </summary>
        public void BeforePageDispose()
        {

        }
        #endregion
    }
}
