using System;
using System.Windows.Forms;

namespace ED_SMR_MLAT_Performance
{
    public partial class Matches : Form
    {
        int matches;

        public Matches()
        {
            InitializeComponent();
        }

        public void ImportarMatches(int Match)
        {
            this.matches = Match;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Matches_Load(object sender, EventArgs e)
        {
            if (matches!=0)
            {
                labelTitle.Text = "File correctly added!";
                labelSubTitle.Text = matches + " points of D-GPS matched\nwith MLAT";
            }
            else
            {
                labelTitle.Text = "Something happened...";
                labelSubTitle.Text = "0 points of D-GPS matched\nwith MLAT";
            }
        }
    }
}
