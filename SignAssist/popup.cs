using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SignAssist
{
    public partial class popup : Form
    {
        List<resultEntry> testResults;
        enum wordEnum { AEROPLANE = 0, ALWAYS, CLEAN, EVERYONE, EQUAL, IMPROVE, LAPTOP, MIDDLE, OKAY, OVER, READ, WHERE }//Create an enumerator which keeps track of the index for each item
        Dictionary<int, string> wordDict;

        public popup(List<resultEntry> results)
        {
            InitializeComponent(); 
            this.CenterToScreen();
            testResults = results;
            wordDict = new Dictionary<int, string>();
            wordDict.Add(0, "Aeroplane");
            wordDict.Add(1, "Always");
            wordDict.Add(2, "Clean");
            wordDict.Add(3, "Everyone");
            wordDict.Add(4, "Equal");
            wordDict.Add(5, "Improve");
            wordDict.Add(6, "Laptop");
            wordDict.Add(7, "Middle");
            wordDict.Add(8, "Okay");
            wordDict.Add(9, "Over");
            wordDict.Add(10, "Read");
            wordDict.Add(11, "Where");
            loadResults();
        }

        public void loadResults()
        {
            int entryIndex = 0;
            int totalOfAverage = 0;

            for (int i = 7; i < ((testResults.Count * 6)+2); i += 6)
            {
                resultEntry temp = testResults[entryIndex];
                int helpPenalty = 0;

                if (temp.helpNeeded) helpPenalty = 10;

                Controls.Find("label" + i, true).First().Text = wordDict[temp.wordIndex];
                Controls.Find("label" + (i + 1), true).First().Text = "" + temp.handshapeScore;
                Controls.Find("label" + (i + 2), true).First().Text = "" + temp.movementScore;
                Controls.Find("label" + (i + 3), true).First().Text = "-" + helpPenalty;
                Controls.Find("label" + (i + 4), true).First().Text = "-" + temp.timePenalty;
                Controls.Find("label" + (i + 5), true).First().Text = "" + temp.overallScore;

                totalOfAverage += temp.overallScore;
                entryIndex++;
            }

            int overallAverage = totalOfAverage / 12;
            if (overallAverage > 90) label80.Text = "A+"+overallAverage;
            else if (overallAverage > 80) label80.Text = "A" + overallAverage;
            else if (overallAverage > 70) label80.Text = "B+" + overallAverage;
            else if (overallAverage > 60) label80.Text = "B" + overallAverage;
            else if (overallAverage > 50) label80.Text = "C" + overallAverage;
            else if (overallAverage > 40) label80.Text = "D" + overallAverage;
            else label80.Text = "F";

        }

    }
}
