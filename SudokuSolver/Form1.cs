using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace SudokuSolver {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
            SetupInput();
            Clear();
        }

        //TODO:
        //Input Grid: UI spaces
        //While working display numbers on panels not numericupdown

        Tile[,] grid;
        Tile cur;
        List<Tile> emptyTiles;

        NumericUpDown[,] inputGrid;
        Label[,] labelGrid;
        Form1 form1;

        int step = 0;
        DateTime startTime;

        Thread thread;
        delegate void UpdateNumericDelegate(NumericUpDown num, int n);
        delegate void UpdateLabelDelegate(Label l, string s);

        void SetupInput() {
            form1 = (Form1)this;
            inputGrid = new NumericUpDown[9, 9];
            NumericUpDown num;
            for(int i = 0; i < 9; i++) {
                for(int j = 0; j < 9; j++) {
                    num = new NumericUpDown();
                    num.Value = 0;
                    num.Minimum = 0;
                    num.Maximum = 9;
                    num.Font = new Font(num.Font.FontFamily, 16);
                    num.Width = 40;
                    num.Location = new Point((num.Width) * i, (num.Height) * j);
                    num.Visible = true;
                    form1.panel1.Controls.Add(num);
                    inputGrid[i, j] = num;
                }
            }
        }

        void InitLabel() {
            labelGrid = new Label[9, 9];
            Label l;
            for(int i = 0; i < 9; i++) {
                for(int j = 0; j < 9; j++) {
                    l = new Label();
                    l.Text = "";
                    l.Font = new Font(l.Font.FontFamily, 16);
                    l.Width = 40;
                    l.Height = 40;
                    l.Visible = false;
                    l.Location = new Point(l.Width * i, l.Height * j);
                    form1.panel1.Controls.Add(l);
                    labelGrid[i, j] = l;
                }
            }
        }

        void InitGrid() {
            grid = new Tile[9, 9];
            for(int i = 0; i < 9; i++) {
                for(int j = 0; j < 9; j++) {
                    grid[i, j] = new Tile(i, j);
                }
            }
        }

        void InitList() {
            emptyTiles = new List<Tile>();
            for(int i = 0; i < 9; i++) {
                for(int j = 0; j < 9; j++) {
                    if(grid[i, j].hasNumber == false) {
                        emptyTiles.Add(grid[i, j]);
                    }
                }
            }
        }

        void SetInput() {
            for(int i = 0; i < 9; i++) {
                for(int j = 0; j < 9; j++) {
                    if(inputGrid[i,j].Value != 0) {
                        grid[i, j].number = (int)inputGrid[i, j].Value;
                        grid[i, j].hasNumber = true;
                    }
                }
            }
        }

        void Setup() {
            Clear();
            InitGrid();
            InitLabel();
            SetInput();
            InitList();

            startTime = DateTime.Now;

            button1.Enabled = false;
            ToogleGridEnabled(false);

            thread = new Thread(Solve);
            thread.Start();
            thread.IsBackground = true;
        }

        void Clear() {
            step = 0;
            label_steps.Text = step.ToString();
            label_time.Text = "00:00:00.0000000";
        }

        void Solve() {
            int index = 0;

            while(!isFinished()) {
                step++;
                StartPosition:;
                Display();
                cur = emptyTiles[index];
                do {
                    if(cur.number < 9) {
                        cur.number++;
                    } else {
                        cur.number = 0;
                        index--;
                        goto StartPosition;
                    }
                } while(!CheckIfOk(cur));
                index++;
                if(index > emptyTiles.Count) {
                    break;
                }
            }
            Display();
            Console.WriteLine("Finished");
            Console.WriteLine("Steps: " + step);
            thread.Abort();
            Finished(); //Does Not work
        }

        bool CheckIfOk(Tile t) {
            int startX = 0;
            int endX = 0;
            int startY = 0;
            int endY = 0;


            //row
            for(int i = 0; i < 9; i++) {
                if(grid[i,t.y].number == t.number && grid[i,t.y] != t) {
                    return false;
                }
            }

            //col
            for(int j = 0; j < 9; j++) {
                if(grid[t.x, j].number == t.number && grid[t.x, j] != t) {
                    return false;
                }
            }

            //box
            if(t.x <= 2) {
                startX = 0;
                endX = 2;
            } else if(t.x <= 5) {
                startX = 3;
                endX = 5;
            } else {
                startX = 6;
                endX = 8;
            }

            if(t.y <= 2) {
                startY = 0;
                endY = 2;
            } else if(t.y <= 5) {
                startY = 3;
                endY = 5;
            } else {
                startY = 6;
                endY = 8;
            }

            for(int i = startX; i <= endX; i++) {
                for(int j = startY; j <= endY; j++) {
                    if(grid[i, j].number == t.number && grid[t.x, j] != t) {
                        return false;
                    }
                }
            }

            return true;
        }

        bool isFinished() {
            int sum = 0;
            for(int i = 0; i < 9; i++) {
                for(int j = 0; j < 9; j++) {
                    if(grid[i,j].number > 0) {
                        sum += grid[i, j].number;
                    } else {
                        return false;
                    }
                }
            }
            if(sum == 45 * 9) {
                return true;
            } else {
                return false;
            }
        }

        void Display() {
            for(int i = 0; i < 9; i++) {
                for(int j = 0; j < 9; j++) {
                    //inputGrid[i, j].Value = grid[i, j].number;
                    inputGrid[i, j].Invoke(new UpdateNumericDelegate(UpdateNumeric), inputGrid[i, j], grid[i,j].number);
                    labelGrid[i,j].Invoke(new UpdateLabelDelegate(UpdateLabel), labelGrid[i,j], grid[i,j].number.ToString());
                }
            }
            label_steps.Invoke(new UpdateLabelDelegate(UpdateLabel), label_steps, step.ToString());
            label_time.Invoke(new UpdateLabelDelegate(UpdateLabel), label_time, (DateTime.Now - startTime).ToString());
        }

        void Finished() {
            Console.WriteLine("Finished outside of thread");
            button1.Enabled = true;
            ToogleGridEnabled(true);
            Clear();
        }

        void UpdateNumeric(NumericUpDown num, int n) {
            num.Value = n;
        }

        void UpdateLabel(Label l, string s) {
            l.Text = s;
        }

        void ToogleGridEnabled(bool nums) {
            for(int i = 0; i < 9; i++) {
                for(int j = 0; j < 9; j++) {
                    inputGrid[i, j].Visible = nums;
                    labelGrid[i, j].Visible = !nums;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            Setup();
        }
    }
}
