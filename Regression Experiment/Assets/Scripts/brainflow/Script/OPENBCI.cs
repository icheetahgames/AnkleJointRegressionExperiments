using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

using Accord;
using Accord.Math;
using brainflow;
using UnityEngine.UI;

using System.Linq;
using System.Text;
using System.IO;

public class OPENBCI : MonoBehaviour
{
    private BoardShim board_shim = null;
    private MLModel concentration = null;
    private int sampling_rate = 0;
    private int[] eeg_channels = null;
    private int board_id;
    private DateTime _startTime;
    private double[,] _specificData;
    
    DataTable dt = new DataTable();
    

    
    [SerializeField] private Text OpenBciCoonnection;
    [SerializeField] private Text TimeStamp;
    [SerializeField] private GameObject Channel_1;
    [SerializeField] private Text Channel_1_text;
    [SerializeField] private GameObject Channel_2;
    [SerializeField] private Text Channel_2_text;
    [SerializeField] private GameObject Channel_3;
    [SerializeField] private Text Channel_3_text;
    [SerializeField] private GameObject Channel_4;
    [SerializeField] private Text Channel_4_text;
    [SerializeField] private Text timeSinceStartUp;
    [SerializeField] private Text _dataSaving;
    [SerializeField] private bool _saveData = false;
    private string title, title_downSampled, title_bandpassFiltered;

    private bool started = false;
    // Start is called before the first frame update
    void Start()
    {
        StartBoard();
        _startTime = DateTime.UtcNow.ToLocalTime();
        _specificData = new double[2, 200];
        
        title = "logs/OpenBci/OpenBci " +
               _startTime.Year.ToString() +
               "-" + _startTime.Month.ToString() +
               "-" + _startTime.Day.ToString() +
               "@" + _startTime.Hour.ToString() +
               "-" + _startTime.Minute.ToString() +
               "-" + _startTime.Second.ToString() +
               ".csv";
                
        title_downSampled = "logs/OpenBci/OpenBci(DownSampled) " +
                _startTime.Year.ToString() +
                "-" + _startTime.Month.ToString() +
                "-" + _startTime.Day.ToString() +
                "@" + _startTime.Hour.ToString() +
                "-" + _startTime.Minute.ToString() +
                "-" + _startTime.Second.ToString() +
                ".csv";
        
        title_bandpassFiltered = "logs/OpenBci/OpenBci(BandPassedFiltered) " +
                                 _startTime.Year.ToString() +
                                 "-" + _startTime.Month.ToString() +
                                 "-" + _startTime.Day.ToString() +
                                 "@" + _startTime.Hour.ToString() +
                                 "-" + _startTime.Minute.ToString() +
                                 "-" + _startTime.Second.ToString() +
                                 ".csv";

        _dataSaving.text = "EMG Data Saving : " + _saveData.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        _dataSaving.text = "EMG Data Saving : " + _saveData.ToString();
        OpenBciCoonnection.text = "OpenBci_Connected : " + board_shim.is_prepared();
        if ((board_shim == null) || (concentration == null))
        {
            return;
        }
        timeSinceStartUp.text = Time.realtimeSinceStartup.ToString();
         int number_of_data_points = sampling_rate * 10 * 60; // The data will be save for 10 min
        
             // 4 second window is recommended for concentration and relaxation calculations
        double[,] data = board_shim.get_current_board_data(number_of_data_points);
        int count = board_shim.get_board_data_count();
        
        // if (data.GetRow(0).Length < number_of_data_points)
        // {
        //     // wait for more data
        //     return;
        // }

        TimeStamp.text = UnixTimeStampToDateTime(data[data.GetColumn(0).Length - 2, data.GetRow(0).Length - 1]).ToString();
        // print("colomns : " +  data.GetRow(0).Length);
        //  print("rows : " +  data.GetColumn(0).Length);
        //print("timeStamp : " + data[data.GetColumn(0).Length - 2, data.GetRow(0).Length - 1]);
        // print("Real Time Stamp : " + UnixTimeStampToDateTime(data[data.GetColumn(0).Length - 2, data.GetRow(0).Length - 1]));
        // print("unity Time : " + DateTime.UtcNow.ToLocalTime());
        
        
        

        
        
        //Channel_1.GetComponent<HealthBar>().SetHealth ((float)data[data.GetColumn(0).Length - 14, data.GetRow(0).Length - 1]);
        Channel_1.GetComponent<HealthBar>().SetHealth ((float)data[data.GetColumn(0).Length - 14, data.GetRow(0).Length - 1]);
        Channel_1_text.text = data[data.GetColumn(0).Length - 14, data.GetRow(0).Length - 1].ToString() + "uVmrs";
        
        Channel_2.GetComponent<HealthBar>().SetHealth ((float)data[data.GetColumn(0).Length - 13, data.GetRow(0).Length - 1]);
        Channel_2_text.text = data[data.GetColumn(0).Length - 13, data.GetRow(0).Length - 1].ToString() + "uVmrs";
        
        Channel_3.GetComponent<HealthBar>().SetHealth ((float)data[data.GetColumn(0).Length - 12, data.GetRow(0).Length - 1]);
        Channel_3_text.text = data[data.GetColumn(0).Length - 12, data.GetRow(0).Length - 1].ToString() + "uVmrs";
        
        Channel_4.GetComponent<HealthBar>().SetHealth ((float)data[data.GetColumn(0).Length - 11, data.GetRow(0).Length - 1]);
        Channel_4_text.text = data[data.GetColumn(0).Length - 11, data.GetRow(0).Length - 1].ToString() + "uVmrs";

        // you can use MEAN, MEDIAN or EACH for downsampling
        double[] DownSampled_sample = DataFilter.perform_downsampling (data.GetRow(0), 4, (int)AggOperations.MEDIAN);
        double[] DownSampled_channel_1 = DataFilter.perform_downsampling (data.GetRow(1), 4, (int)AggOperations.MEDIAN);
        double[] DownSampled_channel_2 = DataFilter.perform_downsampling (data.GetRow(2), 4, (int)AggOperations.MEDIAN);
        double[] DownSampled_channel_3 = DataFilter.perform_downsampling (data.GetRow(3), 4, (int)AggOperations.MEDIAN);
        double[] DownSampled_channel_4 = DataFilter.perform_downsampling (data.GetRow(4), 4, (int)AggOperations.MEDIAN);
        double[] DownSampled_timeStamp = DataFilter.perform_downsampling (data.GetRow(13), 4, (int)AggOperations.MEDIAN);
        
        double[,] data2 = new double[6,DownSampled_sample.Length];
        
        for (int i = 0; i < DownSampled_sample.Length; i++)
        {
            data2[0, i] = DownSampled_sample[i];
            data2[1, i] = DownSampled_channel_1[i];
            data2[2, i] = DownSampled_channel_2[i];
            data2[3, i] = DownSampled_channel_3[i];
            data2[4, i] = DownSampled_channel_4[i];
            data2[5, i] = DownSampled_timeStamp[i];
        }

        
        double[] BandPassFiltered_sample = DataFilter.perform_bandpass (data2.GetRow(0), BoardShim.get_sampling_rate (board_id), 15, 5, 2, (int)FilterTypes.BESSEL, 0.0);
        double[] BandPassFiltered_channel_1 = DataFilter.perform_bandpass (data2.GetRow(1), BoardShim.get_sampling_rate (board_id), 15, 5, 2, (int)FilterTypes.BESSEL, 0.0);
        double[] BandPassFiltered_channel_2 = DataFilter.perform_bandpass (data2.GetRow(2), BoardShim.get_sampling_rate (board_id), 15, 5, 2, (int)FilterTypes.BESSEL, 0.0);
        double[] BandPassFiltered_channel_3 = DataFilter.perform_bandpass (data2.GetRow(3), BoardShim.get_sampling_rate (board_id), 15, 5, 2, (int)FilterTypes.BESSEL, 0.0);
        double[] BandPassFiltered_channel_4 = DataFilter.perform_bandpass (data2.GetRow(4), BoardShim.get_sampling_rate (board_id), 15, 5, 2, (int)FilterTypes.BESSEL, 0.0);
        double[] BandPassFiltered_timeStamp = DataFilter.perform_bandpass (data2.GetRow(5), BoardShim.get_sampling_rate (board_id), 15, 5, 2, (int)FilterTypes.BESSEL, 0.0);
        
        double[,] data_BPF = new double[6,BandPassFiltered_sample.Length];
        
        for (int i = 0; i < BandPassFiltered_sample.Length; i++)
        {
            data_BPF[0, i] = BandPassFiltered_sample[i];
            data_BPF[1, i] = BandPassFiltered_channel_1[i];
            data_BPF[2, i] = BandPassFiltered_channel_2[i];
            data_BPF[3, i] = BandPassFiltered_channel_3[i];
            data_BPF[4, i] = BandPassFiltered_channel_4[i];
            data_BPF[5, i] = BandPassFiltered_timeStamp[i];
        }
        
        if (_saveData)
        {
            DataFilter.write_file (data, title, "w"); // Save data to csv file
            DataFilter.write_file (data2, title_downSampled, "w"); // Save data to csv file
            DataFilter.write_file (data_BPF, title_bandpassFiltered, "w"); // Save data to csv file
        }
        

        // double[] sample = data.GetRow(0);
        // double[] channel_1 = data.GetRow(1);
        // double[] channel_2 = data.GetRow(2);
        // double[] timeStamp = data.GetRow(13);
        // print("samples length : " + sample.Length);
        // double[,] data2 = new double[3,sample.Length];
        // for (int i = 0; i < sample.Length; i++)
        // {
        //     data2[0, i] = sample[i];
        //     data2[1, i] = channel_1[i]; 
        //     data2[1, i] = timeStamp[i];
        // }
        //
        // DataFilter.write_file (data2, "logs/OpenBci (specific data) "+
        //                               _startTime.Year.ToString() +
        //                               "-" + _startTime.Month.ToString() +
        //                               "-" + _startTime.Day.ToString() +
        //                               "@" + _startTime.Hour.ToString() +
        //                               "-" + _startTime.Minute.ToString() +
        //                               "-" + _startTime.Second.ToString() +
        //                               ".csv", "a"); // Save data to csv file
        //
        // // you can use MEAN, MEDIAN or EACH for downsampling
        // double[] filtered_sample = DataFilter.perform_downsampling (sample, 2, (int)AggOperations.MEAN);
        // double[] filtered_channel_1 = DataFilter.perform_downsampling (channel_1, 2, (int)AggOperations.MEAN);
        // double[] filtered_timeStamp = DataFilter.perform_downsampling (timeStamp, 2, (int)AggOperations.MEAN);
        //
        // double[,] data3 = new double[3,filtered_sample.Length];
        // print("filtered_sample length : " + filtered_sample.Length);
        // print("filtered_channel_1 length : " + filtered_channel_1.Length);
        // for (int i = 0; i < filtered_sample.Length; i++)
        // {
        //     data3[0, i] = filtered_sample[i];
        //     data3[1, i] = filtered_channel_1[i]; 
        //     data3[1, i] = filtered_timeStamp[i];
        // }
        //
        //
        //
        // DataFilter.write_file (data3, "logs/OpenBci (downsampled data) "+
        //                                  _startTime.Year.ToString() +
        //                                  "-" + _startTime.Month.ToString() +
        //                                  "-" + _startTime.Day.ToString() +
        //                                  "@" + _startTime.Hour.ToString() +
        //                                  "-" + _startTime.Minute.ToString() +
        //                                  "-" + _startTime.Second.ToString() +
        //                                  ".csv", "a"); // Save data to csv file
        // print("------------------------------------------------------------------------");
    }

    private void OnDestroy()
    {
        if (board_shim != null)
        {
            try
            {
                board_shim.release_session();
                concentration.release();
            }
            catch (BrainFlowException e)
            {
                Debug.Log(e);
            }
            Debug.Log("Brainflow streaming was stopped");
            
        }
    }
    
    public DateTime UnixTimeStampToDateTime( double unixTimeStamp )
    {
        // Unix timestamp is seconds past epoch
        System.DateTime dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds( unixTimeStamp ).ToLocalTime();
        return dtDateTime;
    }

    public void StartBoard()
    {
        try
        {
            BoardShim.set_log_file("brainflow_log.txt");
            BoardShim.enable_dev_board_logger();

            BrainFlowInputParams input_params = new BrainFlowInputParams();
            input_params.serial_port = "COM7";
            board_id = (int)BoardIds.GANGLION_BOARD;
            board_shim = new BoardShim(board_id, input_params);
            board_shim.prepare_session();
            board_shim.start_stream(450000, "file://brainflow_data.csv:w");
            BrainFlowModelParams concentration_params = new BrainFlowModelParams((int)BrainFlowMetrics.CONCENTRATION, (int)BrainFlowClassifiers.REGRESSION);
            concentration = new MLModel(concentration_params);
            concentration.prepare();

            sampling_rate = BoardShim.get_sampling_rate(board_id);
            eeg_channels = BoardShim.get_eeg_channels(board_id);
            Debug.Log("Brainflow streaming was started");
        }
        catch (BrainFlowException e)
        {
            Debug.Log(e);
        }
    }

    public void StopStreaming()
    {
        StopStreaming();
    }
    
    public static DataTable ArraytoDatatable(double[,] array)
    {                 
        DataTable dt = new DataTable();
        for (int i = 0; i < array.GetLength(1); i++)
        {
            dt.Columns.Add("Column" + (i + 1));
        }
        
        for (var i = 0; i < array.GetLength(0); ++i)
        {
            DataRow row = dt.NewRow();
            for (var j = 0; j < array.GetLength(1); ++j)
            {
                row[j] = array[i, j];
            }
            dt.Rows.Add(row);
        }
        return dt;
    }

    public void StartSavingData()
    {
        print("Save OpenBci Records");
        _saveData = true;
    }
    
    public void stopSavingData()
    {
        print("Stop Saving Records");
        _saveData = false;
    }
}