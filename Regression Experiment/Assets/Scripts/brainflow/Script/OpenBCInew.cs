using System;
using System.Collections;
using System.Collections.Generic;
using brainflow;
using UnityEngine;

public class OpenBCInew : MonoBehaviour
{
    private BoardShim board_shim = null;
    private int sampling_rate = 0;
    private string title;
    private DateTime _startTime;
    // Start is called before the first frame update
    void Start()
    {
        _startTime = DateTime.UtcNow.ToLocalTime();
        
        title = "logs/OpenBci " +
                _startTime.Year.ToString() +
                "-" + _startTime.Month.ToString() +
                "-" + _startTime.Day.ToString() +
                "@" + _startTime.Hour.ToString() +
                "-" + _startTime.Minute.ToString() +
                "-" + _startTime.Second.ToString() +
                ".csv";
        
        try
        {
            BoardShim.set_log_file("brainflow_log.txt");
            BoardShim.enable_dev_board_logger();

            BrainFlowInputParams input_params = new BrainFlowInputParams();
            int board_id = (int)BoardIds.SYNTHETIC_BOARD;
            board_shim = new BoardShim(board_id, input_params);
            board_shim.prepare_session();
            board_shim.start_stream(450000, "file://brainflow_data.csv:w");
            sampling_rate = BoardShim.get_sampling_rate(board_id);
            Debug.Log("Brainflow streaming was started");
        }
        catch (BrainFlowException e)
        {
            Debug.Log(e);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (board_shim == null)
        {
            return;
        }
        int number_of_data_points = sampling_rate;
        double[,] data = board_shim.get_current_board_data(number_of_data_points);
        DataFilter.write_file (data, title, "w");
        // check https://brainflow.readthedocs.io/en/stable/index.html for api ref and more code samples
        Debug.Log("Num elements: " + data.GetLength(1));
    }

    // you need to call release_session and ensure that all resources correctly released
    private void OnDestroy()
    {
        if (board_shim != null)
        {
            try
            {
                board_shim.release_session();
            }
            catch (BrainFlowException e)
            {
                Debug.Log(e);
            }
            Debug.Log("Brainflow streaming was stopped");
        }
    }
}
