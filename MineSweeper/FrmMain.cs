using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MineSweeper.World;
using MineSweeper.Helper;
using MineSweeper.Ui;

namespace MineSweeper
{
    public partial class FrmMain : Form
    {
        private MineSweeperWorld mMineSweeper = null;
        private bool IsEnd = false;
        private int mWidth = 10;
        private int mHeight = 10;
        private int mBomb = 10;
        
        public FrmMain()
        {
            InitializeComponent();

            mMineSweeper = new MineSweeperWorld();
            mMineSweeper.OnTraceOut += MineSweeper_OnTraceOut;
            mMineSweeper.OnPlayData += MineSweeper_OnPlayData;
           
        }

        private void NewGame(int width, int height, int bombcount)
        {
            IsEnd = false;
            mMineSweeper.InitBoard(width, height, bombcount);
            plBackGround.Width = (mMineSweeper.Margin * 2) + (mMineSweeper.GridSize * mMineSweeper.GridWidth);
            plBackGround.Height = (mMineSweeper.Margin * 2) + (mMineSweeper.GridSize * mMineSweeper.GridHeight);

            this.ClientSize = new Size(plBackGround.Width, plBackGround.Height + msMenu.Height + 5);

            mMineSweeper.SetGraphics(plBackGround.CreateGraphics());

            mMineSweeper.DrawBoard();
        }

        private void MineSweeper_OnPlayData(object sender, PlayDataEventArgs e)
        {
            if(!IsEnd) {
                if (e.PlayState.Equals("WIN")) {
                    IsEnd = true;
                    using (FrmEndResult frmEnd = new FrmEndResult("You Win!")) {
                        frmEnd.ShowDialog();
                    }
                } else if (e.PlayState.Equals("DEFEAT")) {
                    IsEnd = true;
                    using (FrmEndResult frmEnd = new FrmEndResult("You Defeat!")) {
                        frmEnd.ShowDialog();
                    }
                }
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            NewGame(mWidth, mHeight, mBomb);
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            mMineSweeper.DrawBoard();
        }

        private void MineSweeper_OnTraceOut(object sender, TraceOutEventArgs e)
        {
            TraceOut(e.Message);
        }

        private void TraceOut(string aMessage)
        {
            txbDebug.RunOnUIThread(x =>
            {
                if (x.TextLength > x.MaxLength)
                    x.Clear();
                x.AppendText(string.Format(">> {0}\r\n", aMessage));
            });
        }

        private void TraceOut(string aFormat, params object[] aArgs)
        {
            TraceOut(string.Format(aFormat, aArgs));
        }

        private void BackGround_MouseDown(object sender, MouseEventArgs e)
        {
            //mIsClick = true;
            if (!IsEnd) {
                int x = (e.X - mMineSweeper.Margin) / mMineSweeper.GridSize;
                int y = (e.Y - mMineSweeper.Margin) / mMineSweeper.GridSize;

                if ((x < mMineSweeper.GridWidth) && (y < mMineSweeper.GridHeight)) {
                    switch (e.Button) {
                        case MouseButtons.Left:
                            mMineSweeper.DrawRect(x, y);
                            break;
                        case MouseButtons.Right:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void BackGround_MouseUp(object sender, MouseEventArgs e)
        {
            //mIsClick = false;
            if (!IsEnd) {
                int x = (e.X - mMineSweeper.Margin) / mMineSweeper.GridSize;
                int y = (e.Y - mMineSweeper.Margin) / mMineSweeper.GridSize;

                if ((x < mMineSweeper.GridWidth) && (y < mMineSweeper.GridHeight)) {
                    switch (e.Button) {
                        case MouseButtons.Left:
                            mMineSweeper.SetBoardState(x, y);
                            mMineSweeper.DrawBoard();
                            break;
                        case MouseButtons.Right:
                            if (mMineSweeper.SetBoardState(x, y, true)) {
                                mMineSweeper.DrawBoard();
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void SetGameOption(int w, int h, int b)
        {
            mWidth = w;
            mHeight = h;
            mBomb = b;
        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FrmNewGame frmNewGame = new FrmNewGame(this)) {
                if(frmNewGame.ShowDialog() == DialogResult.OK) {
                    NewGame(mWidth, mHeight, mBomb);
                }
                frmNewGame.Dispose();
            }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
