using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SJF_but_nicer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // todo
        // 1. Debug SRTF Completion and Response Times not updating
        // 2. Integrate Average values in table
        // 3. Deal with unexpected behavior when CPU is given deadtime
        // 4. Display GANTT chart in UI
        // 5. Polish up UI??

        public MainWindow()
        {
            InitializeComponent();
            MessageBox.Show("Good luck trying to parse through the code :)\nI did as best i could trying to write it in a way that's easy to understand");
        }


        /// <summary>
        /// adds a new row to add job information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewJob_Click(object sender, RoutedEventArgs e)
        {
            // Add a new row
            Add_Row();

            // adjust title and opacity if necessary
            if (mainGrid.RowDefinitions.Count == 5)
            {
                // Delete Button
                DeleteRowButton.IsEnabled = true;
                DeleteRowButton.Opacity = 1;

                // Column Titles
                NameTitle.Opacity = 1;
                ArrivalTitle.Opacity = 1;
                BurstTitle.Opacity = 1;

                // Column Outlines
                NameRectangle.Opacity = 1;
                ArrivalRectangle.Opacity = 1;
                BurstRectangle.Opacity = 1;
            }
        }


        /// <summary>
        /// Adds a new row and adjusts any button's placement
        /// </summary>
        private void Add_Row()
        {
            // create a new row and give it some properties
            RowDefinition tempRow = new RowDefinition();
            tempRow.Name = "Middle" + (mainGrid.RowDefinitions.Count - 2);
            tempRow.Height = new GridLength(25);
            mainGrid.RowDefinitions.Add(tempRow);

            // add in a text box into each section of the table
            for (int i = 1; i <= 3; i++)
            {
                // initialize some bois
                TextBox textBox = new TextBox();
                Rectangle rectangle = new Rectangle();
                mainGrid.Children.Add(textBox);
                mainGrid.Children.Add(rectangle);

                // Set the outline for the rectangle
                rectangle.StrokeThickness = 2;
                rectangle.Stroke = new SolidColorBrush(Colors.Black);

                // set the text alignment for the text box
                textBox.TextAlignment = TextAlignment.Center;

                // Put the elements in the right place
                Grid.SetColumn(textBox, i);
                Grid.SetColumn(rectangle, i);
                Grid.SetRow(textBox, mainGrid.RowDefinitions.Count - 3);
                Grid.SetRow(rectangle, mainGrid.RowDefinitions.Count - 3);

                // move the buttons to the correct location
                Grid.SetRow(RunButton, mainGrid.RowDefinitions.Count - 2);
                Grid.SetRow(AddNewJob, mainGrid.RowDefinitions.Count - 2);
                Grid.SetRow(DeleteRowButton, mainGrid.RowDefinitions.Count - 2);
            }
        }


        /// <summary>
        /// it takes the average of the array.
        /// </summary>
        /// <param name="values">the values to average. </param>
        /// <returns> the average. </returns>
        private double Average(int[] values)
        {
            // I miss the python builtin sum() function :c
            double sum = 0;
            foreach (int value in values)
                sum += value;

            return sum / values.Length;
        }


        /// <summary>
        /// Deletes a row at the given index and every element in that row
        /// (Definitely not code that I copied, pasted, and slightly modified from ChatGPT, also it glitchy)
        /// </summary>
        /// <param name="rowIndex"></param>
        private void DeleteRow(int rowIndex)
        {
            // this guy is a little glitchy

            // Remove the elements within the row
            for (int i=0; i < mainGrid.Children.Count; i++)
            {
                int elementRowIndex = Grid.GetRow(mainGrid.Children[i]);
                if (elementRowIndex == rowIndex)
                {
                    mainGrid.Children.Remove(mainGrid.Children[i]);
                }
            }

            // Remove the row definition
            mainGrid.RowDefinitions.RemoveAt(rowIndex);
        }


        /// <summary>
        /// An overload for DeleteRow which I will use more than the base method
        /// </summary>
        private void DeleteRow()
        {
            DeleteRow(mainGrid.RowDefinitions.Count - 3);
        }


        /// <summary>
        /// Removes a row and its rectangles and text boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            // Removes a row (so long as it doesn't fuck anything up*)
            // *terms and conditions may apply
            if (mainGrid.RowDefinitions.Count >= 5)
            {
                DeleteRow();
            }

            // Adjust Opacity if necessary
            if (mainGrid.RowDefinitions.Count == 4)
            {
                // For the delete button
                DeleteRowButton.IsEnabled = false;
                DeleteRowButton.Opacity = 0;

                // Title Text Boxes
                NameTitle.Opacity = 0;
                ArrivalTitle.Opacity = 0;
                BurstTitle.Opacity = 0;

                // Column Outlines
                NameRectangle.Opacity = 0;
                ArrivalRectangle.Opacity = 0;
                BurstRectangle.Opacity = 0;
            }
        }


        /// <summary>
        /// UNFINISHED- Takes entered data, processes it, and displays proper answers when completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            // import data from text boxes
            int lengths = mainGrid.RowDefinitions.Count - 4;
            TextBox[] nameBoxes = new TextBox[lengths];
            TextBox[] arrivalBoxes = new TextBox[lengths];
            TextBox[] burstBoxes = new TextBox[lengths];

            foreach (UIElement element in mainGrid.Children)
            {
                if (element is TextBox)
                {
                    if (Grid.GetColumn(element) == 1)
                        nameBoxes[Grid.GetRow(element) - 2] = (TextBox) element;

                    else if (Grid.GetColumn(element) == 2)
                        arrivalBoxes[Grid.GetRow(element) - 2] = (TextBox) element;

                    else if (Grid.GetColumn(element) == 3)
                        burstBoxes[Grid.GetRow(element) - 2] = (TextBox) element;
                }
            }



            // convert data into an array and dict of jobs
            Dictionary<string, Job> jobs = new Dictionary<string, Job>();
            Job[] jobList = new Job[lengths];

            for (int i = 0; i < lengths; i++)
            {
                Job temp = new Job(nameBoxes[i].Text, Convert.ToInt32(arrivalBoxes[i].Text), Convert.ToInt32(burstBoxes[i].Text));
                jobs.Add(temp.Name, temp);
                jobList[i] = temp;
            }





            // process and sort data

            // FOR SRTF
            // List<ValueTuple<string, int>> gantt = SRTF(jobList, jobs);
            //

            // FOR SJF
            SJF(jobList);
            int[] waitTimes = SJFCalculateWaitTimes(jobList);
            int[] completionTimes = SJFCalculateCompletionTimes(jobList);
            //

            // display data in UI
            for (int i = 0; i < 4; i++)
                mainGrid.ColumnDefinitions.Add(new ColumnDefinition());

            Add_Row();

            // change the UI (delete buttons and show title boxes)
            AddNewJob.IsEnabled = false;
            AddNewJob.Opacity = 0;

            DeleteRowButton.Opacity = 0;
            DeleteRowButton.IsEnabled = false;

            RunButton.IsEnabled = false;
            RunButton.Opacity = 0;

            CompletionTitle.Opacity = 1;
            Grid.SetColumn(CompletionTitle, 4);
            CompletionRectangle.Opacity = 1;
            Grid.SetColumn(CompletionRectangle, 4);

            TurnaroundTitle.Opacity = 1;
            Grid.SetColumn(TurnaroundTitle, 5);
            TurnaroundRectangle.Opacity = 1;
            Grid.SetColumn(TurnaroundRectangle, 5);

            WaitTitle.Opacity = 1;
            Grid.SetColumn(WaitTitle, 6);
            WaitRectangle.Opacity = 1;
            Grid.SetColumn(WaitRectangle, 6);

            ResponseTitle.Opacity = 1;
            Grid.SetColumn(ResponseTitle, 7);
            ResponseRectangle.Opacity = 1;
            Grid.SetColumn(ResponseRectangle, 7);

            int[] completions = new int[lengths];
            int[] turnarounds = new int[lengths];
            int[] waits = new int[lengths];
            int[] responses = new int[lengths];

            // fill the table and give each spot some data
            for (int i = 0; i < lengths; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Rectangle rectangle = new Rectangle();
                    TextBlock textBlock = new TextBlock();

                    rectangle.Stroke = new SolidColorBrush(Colors.Black);
                    rectangle.StrokeThickness = 2;

                    textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                    textBlock.VerticalAlignment = VerticalAlignment.Center;

                    // FOR SJF
                    if (j == 0)
                        textBlock.Text = Convert.ToString(completionTimes[i]);
                    else if (j == 2)
                        textBlock.Text = Convert.ToString(waitTimes[i]);
                    //

                    /* FOR SRTF
                    string name = jobList[i].Name;
                    if (j == 0)
                    {
                        completions[i] = jobs[name].CompletionTime;
                        textBlock.Text = Convert.ToString(completions[i]);
                    }
                    else if (j == 1)
                    {
                        turnarounds[i] = jobs[name].TurnaroundTime;
                        textBlock.Text = Convert.ToString(turnarounds[i]);
                    }
                    else if (j == 2)
                    {
                        waits[i] = jobs[name].WaitTime;
                        textBlock.Text = Convert.ToString(waits[i]);
                    }
                    else if (j == 3)
                    {
                        responses[i] = jobs[name].ResponseTime;
                        textBlock.Text = Convert.ToString(responses[i]);
                    }
                    */
                    //

                    mainGrid.Children.Add(rectangle);
                    mainGrid.Children.Add(textBlock);

                    Grid.SetRow(rectangle, i + 2);
                    Grid.SetColumn(rectangle, j + 4);

                    Grid.SetRow(textBlock, i + 2);
                    Grid.SetColumn(textBlock, j + 4);

                }

            }

            // display averages for each category

            // FOR SRTF
            /*
            MessageBox.Show("I'm lazy so you're getting the averages in popup format.\nI've already put in a lot of effort to this");
            MessageBox.Show("Average Completion Time: " + Convert.ToString(Average(completions)));
            MessageBox.Show("Average Turnaround Time: " + Convert.ToString(Average(turnarounds)));
            MessageBox.Show("Average Wait Time: " + Convert.ToString(Average(waits)));
            MessageBox.Show("Average Response Time: " + Convert.ToString(Average(responses)));
            */
            //

            // FOR SJF
            MessageBox.Show("I'm lazy so you're getting the averages in popup format.\nI've already put in a lot of effort to this");
            MessageBox.Show("Average Completion Time: " + Convert.ToString(Average(completionTimes)));
            MessageBox.Show("Average Wait Time: " + Convert.ToString(Average(waitTimes)));


            // work on displaying GANTT chart here (will maybe do later idk its already a lot of code)

        }


        /// <summary>
        /// Run SJF on all jobs in the "ready queue"
        /// </summary>
        /// <param name="jobs"> The jobs to sort </param>
        private void SJF(Job[] jobs)
        {
            SJFSort(jobs, 0, jobs.Length - 1);
        }


        /// <summary>
        /// More SJF overloads
        /// </summary>
        /// <param name="jobs"></param>
        /// <returns></returns>
        private List<Job> SJF(List<Job> jobs)
        {
            Job[] jobs2 = jobs.ToArray();
            SJF(jobs2);
            return jobs2.ToList();
        }


        /// <summary>
        /// Calculate the completion time for all jobs in the queue (for SJF or FCFS)
        /// </summary>
        /// <param name="jobs"> An array of jobs to calculate the completion time for </param>
        /// <returns> An array of completion times </returns>
        private int[] SJFCalculateCompletionTimes(Job[] jobs)
        {
            int[] result = new int[jobs.Length];
            result[0] = jobs[0].BurstTime;

            for (int i = 1; i < jobs.Length; i++)
                result[i] = jobs[i].BurstTime + result[i - 1];

            return result;
        }


        /// <summary>
        /// Calculate the wait time for all jobs in the queue (SJF or FCFS)
        /// </summary>
        /// <param name="jobs">An array of jobs to calculate the wait time for</param>
        /// <returns> An array of wait times </returns>
        private int[] SJFCalculateWaitTimes(Job[] jobs)
        {
            int[] result = new int[jobs.Length];
            result[0] = 0;

            for (int i = 1; i < jobs.Length; i++)
                result[i] = jobs[i].BurstTime + result[i - 1];

            return result;
        }


        /// <summary>
        /// Its just Merge Sort.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private Job[] SJFSort(Job[] array, int left, int right)
        {
            if (left < right)
            {
                int middle = left + (right - left) / 2;
                SJFSort(array, left, middle);
                SJFSort(array, middle + 1, right);
                SJFMerge(array, left, middle, right);
            }
            return array;
        }


        /// <summary>
        /// just like the actual merging of mergesort, thats all.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="left"></param>
        /// <param name="middle"></param>
        /// <param name="right"></param>
        private void SJFMerge(Job[] array, int left, int middle, int right)
        {
            var leftArrayLength = middle - left + 1;
            var rightArrayLength = right - middle;
            var leftTempArray = new Job[leftArrayLength];
            var rightTempArray = new Job[rightArrayLength];
            int i, j;
            for (i = 0; i < leftArrayLength; ++i)
                leftTempArray[i] = array[left + i];
            for (j = 0; j < rightArrayLength; ++j)
                rightTempArray[j] = array[middle + 1 + j];
            i = 0;
            j = 0;
            int k = left;
            while (i < leftArrayLength && j < rightArrayLength)
            {
                if (leftTempArray[i].BurstTime <= rightTempArray[j].BurstTime)
                {
                    array[k++] = leftTempArray[i++];
                }
                else
                {
                    array[k++] = rightTempArray[j++];
                }
            }
            while (i < leftArrayLength)
            {
                array[k++] = leftTempArray[i++];
            }
            while (j < rightArrayLength)
            {
                array[k++] = rightTempArray[j++];
            }
        }


        /// <summary>
        /// splits an array of jobs into two arrays, with array[index] being the first entry of the second array
        /// </summary>
        /// <param name="array"> the array to be split </param>
        /// <param name="index"> the index of the first element to be after the split </param>
        /// <returns> both subarrays as lists </returns>
        private List<Job>[] split(Job[] array, int index)
        {
            List<Job> first = new List<Job>();
            List<Job> second = new List<Job>();
            List<Job>[] thing = new List<Job>[2];
            for (int i=0; i < array.Length; ++i)
            {
                if (i < index)
                {
                    Job test = array[i];
                    first.Add(array[i]);
                }

                else
                {
                    Job test = array[i];
                    second.Add(array[i]);
                }

            }
            thing[0] = first;
            thing[1] = second;

            return thing;
        }

        
        /// <summary>
        /// Determine the ordering of jobs  for SRTF
        /// </summary>
        /// <param name="jobArray">The array of jobs which is to be calculated </param>
        /// <returns> An array of jobs, each index representing one unit of time and which process was calculated at that time </returns>
        List<ValueTuple<string, int>> SRTF(Job[] jobArray, Dictionary<string, Job> jobDict)
        {
            // sort the jobs by burst time
            SJF(jobArray);

            // sort the jobs by arrival time
            SRTFSort(jobArray, 0, jobArray.Count() - 1);

            // organize the jobs into buckets (one bucket per arrival time)
            List<List<Job>> jobsByArrival = new List<List<Job>>();
            int i = 1;
            while (i < jobArray.Length)
            {
                // if the arrival time of the current job is not the same as the previous, then add that chunk as a bucket into jobsByArrival
                if (jobArray[i].ArrivalTime != jobArray[i-1].ArrivalTime)
                {
                    List<Job>[] temp = split(jobArray, i);
                    jobsByArrival.Add(temp[0]);
                    jobArray = temp[1].ToArray();
                    i = 1;
                }
                else
                    i++;
            }

            int timeSlice = 0;
            i = 0;
            // add the first jobs
            List<ValueTuple<string, int>> gantt = new List<ValueTuple<string, int>>();
            List<Job> readyQueue = jobsByArrival[0];
            jobsByArrival.RemoveAt(0);
            int time = readyQueue[0].ArrivalTime;
            // run the current process until either the current job runs out or more jobs arrive
            while (jobsByArrival.Count > 0)
            {
                // run the current time
                timeSlice = Math.Min(readyQueue[0].RemainingTime, jobsByArrival[0][0].ArrivalTime - time);
                if (readyQueue[0].ResponseTime < 0)
                    readyQueue[0].ResponseTime = time;
                // THE RESPONSE TIME ISN'T UPDATING WTF (watch my slow descent into madness)
                readyQueue[0].RemainingTime -= timeSlice;
                
                time += timeSlice;

                // if the current job completes
                if (readyQueue[0].RemainingTime == 0)
                {
                    readyQueue[0].CompletionTime = time;
                    jobDict[readyQueue[0].Name] = readyQueue[0];
                    readyQueue.RemoveAt(0);
                    // AAAAAAAAAAAAAAAAAAAAAAAAAAAAAA WHY ISN'T THE COMPLETED TIME UPDATING (the descent is growing faster)
                }
                else if (time == jobsByArrival[0][0].ArrivalTime)
                {
                    // add the newly arriving jobs into the ready queue
                    int middle = readyQueue.Count;
                    readyQueue.AddRange(jobsByArrival[0]);
                    jobsByArrival.RemoveAt(0);

                    // merge the two (already sorted) lists to get one good queue
                    SRTFMerge(readyQueue , 0, middle, readyQueue.Count - 1);


                }

                // READY THE GANTT CHART
                gantt.Add((readyQueue[0].Name, timeSlice));
            }

            return gantt;
        }
       

        /// <summary>
        /// Its just merge sort.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private Job[] SRTFSort(Job[] array, int left, int right)
        {
            if (left < right)
            {
                int middle = left + (right - left) / 2;
                SRTFSort(array, left, middle);
                SRTFSort(array, middle + 1, right);
                SRTFMerge(array, left, middle, right);
            }
            return array;
        }


        /// <summary>
        /// It's just the "merge" part of merge sort, but for 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="left"></param>
        /// <param name="middle"> The piece which denotes the split between the two sorts </param>
        /// <param name="right"></param>
        private void SRTFMerge(Job[] array, int left, int middle, int right)
        {
            var leftArrayLength = middle - left + 1;
            var rightArrayLength = right - middle;
            var leftTempArray = new Job[leftArrayLength];
            var rightTempArray = new Job[rightArrayLength];
            int i, j;
            for (i = 0; i < leftArrayLength; ++i)
                leftTempArray[i] = array[left + i];
            for (j = 0; j < rightArrayLength; ++j)
                rightTempArray[j] = array[middle + 1 + j];
            i = 0;
            j = 0;
            int k = left;
            while (i < leftArrayLength && j < rightArrayLength)
            {
                if (leftTempArray[i].ArrivalTime <= rightTempArray[j].ArrivalTime)
                {
                    array[k++] = leftTempArray[i++];
                }
                else
                {
                    array[k++] = rightTempArray[j++];
                }
            }
            while (i < leftArrayLength)
            {
                array[k++] = leftTempArray[i++];
            }
            while (j < rightArrayLength)
            {
                array[k++] = rightTempArray[j++];
            }
        }


        /// <summary>
        /// an overload of SRTFMerge for Lists
        /// </summary>
        /// <param name="array"></param>
        /// <param name="left"></param>
        /// <param name="middle"></param>
        /// <param name="right"></param>
        private void SRTFMerge(List<Job> array, int left, int middle, int right)
        {
            var leftArrayLength = middle - left + 1;
            var rightArrayLength = right - middle;
            var leftTempArray = new Job[leftArrayLength];
            var rightTempArray = new Job[rightArrayLength];
            int i, j;
            for (i = 0; i < leftArrayLength; ++i)
                leftTempArray[i] = array[left + i];
            for (j = 0; j < rightArrayLength; ++j)
                rightTempArray[j] = array[middle + 1 + j];
            i = 0;
            j = 0;
            int k = left;
            while (i < leftArrayLength && j < rightArrayLength)
            {
                if (leftTempArray[i].ArrivalTime <= rightTempArray[j].ArrivalTime)
                {
                    array[k++] = leftTempArray[i++];
                }
                else
                {
                    array[k++] = rightTempArray[j++];
                }
            }
            while (i < leftArrayLength)
            {
                array[k++] = leftTempArray[i++];
            }
            while (j < rightArrayLength)
            {
                array[k++] = rightTempArray[j++];
            }
        }
    }
}
