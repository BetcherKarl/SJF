using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJF_but_nicer
{
    internal class Job
    {
        // private variable
        private int arrivalTime = int.MaxValue;
        private int burstTime = int.MaxValue;

        // attributes
        public string Name
        { get; set; }
        public int ArrivalTime
        { get => arrivalTime; }
        public int BurstTime
        { get => burstTime; }
        public int RemainingTime
        { get; set; }
        public int CompletionTime
        { get; set; }
        public int ResponseTime
        { get; set; }
        public int TurnaroundTime
        {
            get
            {
                return CompletionTime - ArrivalTime;
            }
        }
        public int WaitTime
        {
            get
            {
                return TurnaroundTime - BurstTime;
            }
        }

        // constructors
        public Job(string name, int arrival, int burst)
        {
            this.Name = name;
            if (arrival >= 0 && burst > 0)
            {
                arrivalTime = arrival;
                burstTime = burst;
                RemainingTime = burst;
                ResponseTime = int.MinValue;
            }
            else
            {
                throw new Exception("" + arrival + "or " + burst + " is not greater than 0.");
            }
        }

        public Job(string name, int burst) : this(name, 0, burst)
        { }

        // methods
        public bool RunProcess()
        {
            if (RemainingTime >= 1)
                RemainingTime--;

            return isDone();
        }

        public bool isDone()
        {
            if (RemainingTime >= 1)
                return false;
            else
                return true;
        }
    }
}
