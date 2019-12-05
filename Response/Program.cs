using System;
using System.Collections;
using System.Collections.Generic;

namespace Response
{
    class Program
    {
        static void Main(string[] args)
        {
            // Creating an ArrayList 
            ArrayList myList = new ArrayList();

            Console.WriteLine("Enter the Start Time:"); //Getting the start time from the user
            string start = Console.ReadLine();
         

            Console.WriteLine("Enter the End Time:"); //Getting the end time from the user
            string end = Console.ReadLine();
         

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Measures to correct the users input
            //If the hour is not two digits (58 is character : in ascii)
            if (start.Length == 4 && (int)start[1] == 58)
                start = start.Insert(0, "0");
            if (end.Length == 4 && (int)end[1] == 58)
                end = end.Insert(0, "0");

            //If the minute is not two digits (58 is character : in ascii)
            if (start.Length == 4 && (int)start[2] == 58)
                start = start.Insert(3, "0");
            if (end.Length == 4 && (int)end[2] == 58)
                end = end.Insert(3, "0");

            //If both the hour and minute are one digit.
            if (start.Length == 3)
            {
                start = start.Insert(0, "0");
                start = start.Insert(3, "0");
            }
            if (end.Length == 3)
            {
                end = end.Insert(0, "0");
                end = end.Insert(3, "0");
            }

            while (!SyntaxChecker(start))
            {
                Console.WriteLine("You Entered An Invalid Start Time. Try The Format 07:43 Please:");
                Console.Write("StartTime is:");
                start = Console.ReadLine();
            }

            while (!SyntaxChecker(end))
            {
                Console.WriteLine("You Entered An Invalid End Time. Try The Format 09:26 Please:");
                Console.Write("EndTime is:");
                end = Console.ReadLine();
            }
           
            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

            myList = TimeSeperator(start, end); //Calling the TimeSeperator method on users inputs.

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Tests
            /*Tests for the time which EndTime is Greater than StartTime and also in the same interval*/
            //myList = TimeSeperator("01:09", "03:13");
            //myList = TimeSeperator("07:04", "07:58");
            //myList = TimeSeperator("11:54", "22:13");

            /*Tests for the time which EndTime is Greater than StartTime but not in the same interval*/
            //myList = TimeSeperator("07:59", "22:23"); //1,2
            //myList = TimeSeperator("04:19", "23:44"); //0,2
            //myList = TimeSeperator("01:09", "07:23"); //0,1

            /*Tests for the time which StartTime is Greater than EndTime, whether they are in the same interval or not*/
            //myList = TimeSeperator("05:00", "04:56"); //0,0
            //myList = TimeSeperator("06:47", "06:43"); //1,1
            //myList = TimeSeperator("23:11", "21:43"); //2,2
            //myList = TimeSeperator("07:00", "04:59"); //1,0
            //myList = TimeSeperator("20:50", "07:33"); //2,1    
            //myList = TimeSeperator("19:43", "02:03"); //2,0          

            //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            for (int i = 0; i < myList.Count; i++) //Printing the results for the user.                      
                Console.WriteLine(myList[i]);
                     
            Console.ReadKey();
        }


        //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%      
        /// <summary>
        /// This method seperates the users input into the given ranges. The logic works in this way:
        /// Find the interval in the ranges, which the startTime is in. If the EndTime is less than the end of the interval,
        /// in oter words, if the interval [StartTime, EndTime] fits inside one of the intervals in the ranges, 
        /// then the answer is "StartTime, EndTime". Otherwise,
        /// we contain all the intervals in the ranges, until we find an interval that EndTime lies in.
        /// </summary>
        /// <param name="StartTime">The start time taken from the user</param>
        /// <param name="EndTime">The end time taken from the user</param>
        /// <returns>The result that is shown to the user.</returns>
        private static ArrayList TimeSeperator(string StartTime, string EndTime)
        {
            // timePeriod is an ArrayList with at most (n + 1) elements, as we break the [StartTime, EndTime] into ranges.(n=3 in this case) 
            // n is the number of intervals in ranges. This happens when StartTime and EndTime are both at the same interval in the ranges. 
            ArrayList timePeriod = new ArrayList(); 
            string []ranges = GetRanges(); //Initialize the ranges
            string[] firstParts = FirstPartCalculator(ranges); //Calculating the first parts of ranges.
            string[] secondParts = SecondPartCalculator(ranges); //Calculating the second parts of ranges.
            int StartTimeIndex = FindRange(StartTime, firstParts, secondParts); //The index of the range which StartTime is in.
            string range = ranges[StartTimeIndex]; //The range which StartTime is in.
            string[] times = IntervalSeperator(range); //Cutting the string range in half.

            //If EndTime is less than the end of the interval, and EndTime is greater
            //than StartTime then the answer is the same interval with no changes.
            if (TimeCompare(times[1], EndTime) && TimeCompare(EndTime, StartTime)) 
            {
                timePeriod.Add(StartTime + ", " + EndTime);
                return timePeriod;
            }
            else //The time when EndTime passes one interval in the ranges, and timePeriod have more than one element.
            {              
                int EndTimeIndex = FindRange(EndTime, firstParts, secondParts); //the index of the range which EndTime is in.
                 //Console.WriteLine(EndTimeIndex);
                 //If the EndTime lies in an interval next to the interval which StartTime lies in. [x to 24]
                if (EndTimeIndex > StartTimeIndex) 
                {
                    string EndInterval; //Used to determine the end of the interval in each period.
                    string StartInterval; //Used to determine the start of the interval in each period.
                    for (int i = StartTimeIndex; i < EndTimeIndex + 1; i++)
                    {
                        //If we didn't reach the interval in ranges that EndTime is in, then
                        //StartInterval and EndInterval are first and second parameter in the ranges.
                        if (i < EndTimeIndex)
                        {
                            StartInterval = firstParts[i];
                            EndInterval = secondParts[i];
                        }
                          
                        //If we reached the interval in ranges that EndTime is in, then EndInterval is the same as EndTime.
                        else
                        {
                            StartInterval = firstParts[i];
                            EndInterval = EndTime;
                        }

                        //If we are calculating the timePeriod for the first time, then StartInterval is the same as StartTime
                        if (i == StartTimeIndex) 
                        {
                            StartInterval = StartTime;
                        }

                        timePeriod.Add(StartInterval + ", " + EndInterval); //Add the string to the answer.                     
                    }
                }

                else //If EndTimeIndex is less than or equal to the StartTimeIndex.
                {
                    /* We go from the interval that StartTime lies in, to the last interval in the ranges. Then we go from 
                     the first interval in the ranges, to the interval that EndTime lies in.*/

                    string EndInterval; //Used to determine the end of the interval in each period.
                    string StartInterval; //Used to determine the start of the interval in each period.                                   

                    //Going from the interval that StartTime lies in, to the last interval in the ranges.
                    for (int i = StartTimeIndex; i < ranges.Length; i++)
                    {                                                                  
                        StartInterval = firstParts[i]; //StartInterval and EndInterval are first and second parameter in the ranges.
                        EndInterval = secondParts[i];
                      
                        //If we are calculating the timePeriod for the first time, then StartInterval is the same as StartTime
                        if (i == StartTimeIndex)
                        {
                            StartInterval = StartTime;
                        }

                        timePeriod.Add(StartInterval + ", " + EndInterval); //Add the string to the answer. 
                    }

                    //Going from the first interval in the ranges, to the interval that EndTime lies in.
                    for (int i = 0; i < EndTimeIndex + 1; i++)
                    {
                        //If we didn't reach the interval in ranges that EndTime is in, then
                        //StartInterval and EndInterval are first and second parameter in the ranges.
                        if (i < EndTimeIndex)
                        {
                            StartInterval = firstParts[i];
                            EndInterval = secondParts[i];
                        }

                        //If we reached the interval in ranges that EndTime is in, then EndInterval is the same as EndTime.
                        else
                        {
                            StartInterval = firstParts[i];
                            EndInterval = EndTime;
                        }                      

                        timePeriod.Add(StartInterval + ", " + EndInterval); //Add the string to the answer.  
                    }
                }

            }

            return timePeriod;
        }
        
        #region Private Helpers
        /// <summary>
        /// Determines the ranges that we divide the user input into them.
        /// </summary>
        /// <returns>The ranges for seperating the user input</returns>
        private static string[] GetRanges()
        {
            string []ranges = { "00:00 - 06:00", "06:00 - 08:00", "08:00 - 24:00" };
            return ranges;
        }

        /// <summary>
        /// This method will seperate the first and second part of a string interval.
        /// </summary>
        /// <param name="interval">The interval string that we want to seperate</param>
        /// <returns>An string array as the seperated strings</returns>
        private static string[] IntervalSeperator(string interval)
        {
            string[] intervals = new string[2]; //A string array in length 2

            intervals[0] = interval.Substring(0, interval.IndexOf('-')); //Retrieve the substring before '-'
            intervals[1] = interval.Substring(interval.IndexOf('-') + 1); //Retrieve the substring after '-'

            intervals[0] = intervals[0].Replace(" ", ""); //Removing the spaces
            intervals[1] = intervals[1].Replace(" ", "");

            return intervals;
        }

        /// <summary>
        /// Seperates the first part of intervals in ranges.
        /// </summary>
        /// <param name="ranges">The ranges which are given to the method as a string array.</param>
        /// <returns>An array of strings which contains the first part of the ranges</returns>
        private static string[] FirstPartCalculator(string[] ranges)
        {
            String[] first = new string[ranges.Length]; //first contains the first part of the intervals.  

            for (int i = 0; i < ranges.Length; i++) //Calculating the array first 
            {
                string[] parts = IntervalSeperator(ranges[i]); //Seperate the string and put the first part in first
                first[i] = parts[0];               
            }
            return first;
        }

        /// <summary>
        /// Seperates the second part of intervals in ranges.
        /// </summary>
        /// <param name="ranges">The ranges which are given to the method as a string array.</param>
        /// <returns>An array of strings which contains the second part of the ranges</returns>
        private static string[] SecondPartCalculator(string[] ranges)
        {            
            String[] second = new string[ranges.Length]; //second contains the second part of the intervals.

            for (int i = 0; i < ranges.Length; i++) //Calculating the array first 
            {
                string[] parts = IntervalSeperator(ranges[i]);  //Seperate the string and put the second part in second          
                second[i] = parts[1];      
            }
            return second;
        }

        /// <summary>
        /// This method finds the range of a time such as "02:00" in the ranges defined.
        /// </summary>
        /// <param name="time">The time specified</param>
        /// <param name="ranges">The ranges that we want to detrmine the position of time among them.</param>
        /// <returns>An integer indicating the place of a specific time in ranges.</returns>
        private static int FindRange(string time, string []first, string[] second)
        {
          

            int index = 0; //Used to store the place of the time in ranges.
            for (int i = 0; i < first.Length; i++)
            {
                if (time == first[i]) //If the first part of an interval from the ranges is equal to the time
                {
                    if (TimeCompare(second[i], time)) //If the time is less than second[i], it means we found what range the time is in.
                        index = i;
                }
                //If time is greater than the first part and less than the second part then we found the range.
                else if (TimeCompare(time, first[i]) && TimeCompare(second[i], time))
                    index = i;
            }
            return index;
        }

        /// <summary>
        /// Returns true, if the first argument is greater than the second argument.
        /// </summary>
        /// <param name="time1">First time</param>
        /// <param name="time2">Second time</param>
        /// <returns>True, if time1 > time2. False, if time2 > time1.</returns>
        private static Boolean TimeCompare(string time1, string time2)
        {
            //Seperating the hour and minute part in the first argument
            string hourPart1 = time1.Substring(0, time1.IndexOf(':')); //Retrieve the substring before ':'
            string minutePart1 = time1.Substring(time1.IndexOf(':') + 1); //Retrieve the substring after ':'

            //Seperating the hour and minute part in the second argument
            string hourPart2 = time2.Substring(0, time2.IndexOf(':')); //retrieve the substring before ':'
            string minutePart2 = time2.Substring(time2.IndexOf(':') + 1); //retrieve the substring after ':'

            hourPart1 = hourPart1.Replace(" ", ""); //Removing the spaces
            minutePart1 = minutePart1.Replace(" ", "");
            hourPart2 = hourPart2.Replace(" ", "");
            minutePart2 = minutePart2.Replace(" ", "");
            
            //Converting strings to integers for calculation purposes.
            int time1hours = Convert.ToInt32(hourPart1);
            int time1minutes = Convert.ToInt32(minutePart1);
            int time2hours = Convert.ToInt32(hourPart2);
            int time2minutes = Convert.ToInt32(minutePart2);

            if (time1hours > time2hours) //If the hour part of time1 is greater, then return true. 
                return true;
            else if (time1hours == time2hours) //If the hours are equal, then compare the minutes.
            {
                if (time1minutes > time2minutes) //If the minute part of time1 is greater then return true.
                    return true;
            }  
            
            return false;
        }

        /// <summary>
        /// This method returns true if the syntax of the user input is correct.
        /// </summary>
        /// <param name="input">The user input could be StartTime or EndTime.</param>
        /// <returns>True if the syntax is right. False if the syntax is not correct.</returns>
        private static Boolean SyntaxChecker(string input)
        {
            /*
             * all the characters should be between 48 and 58 in the ascii codes
             * third character should be ':' 
             * length should be 5
             * first character can be 0 to 2
             * fourth character can be 0 to 5
             */
            if (input.Length != 5)
                return false;

            for (int i = 0; i < input.Length; i++) //If one of the characters are invalid, then return false.
            {
                if ((int)input[i] < 48 || (int)input[i] > 58)
                {
                    return false;
                }
            }

            if (input[2] == ':' && (int)input[0] < 51 && (int)input[0] > 47 && (int)input[3] < 54 && (int)input[3] > 47)
            {
                if (input[0] == '2')
                {
                    if ((int)input[1] > 51) //If the second character is more than 3 return false.
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }


#endregion
    }

}
