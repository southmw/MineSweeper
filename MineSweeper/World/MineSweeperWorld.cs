using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.World
{
    public class MineSweeperWorld
    {
        /// <summary>
        /// 로그 이벤트
        /// </summary>
        public event TraceOutEventHandler OnTraceOut;

        /// <summary>
        /// 플레이 상태 이벤트
        /// </summary>
        public event PlayDataEventHandler OnPlayData;

        /// <summary>
        /// 지뢰 카운트 숫자 색상 테이블
        /// </summary>
        private readonly Brush[] BombCountColors = { Brushes.Blue , Brushes.Green, Brushes.Red, Brushes.Yellow, Brushes.Orange, Brushes.Brown, Brushes.Violet, Brushes.Gold };

        /// <summary>
        /// 그리드 가로 크기
        /// </summary>
        public int GridWidth { get; private set; } = 10;

        /// <summary>
        /// 그리드 세로 크기
        /// </summary>
        public int GridHeight { get; private set; } = 10;

        /// <summary>
        /// 그리드 사이즈
        /// </summary>
        public int GridSize { get; private set; } = 25;

        /// <summary>
        /// 그리드 여백 사이즈
        /// </summary>
        public int Margin { get; private set; } = 5;

        /// <summary>
        /// 지뢰 숫자
        /// </summary>
        public int BombCount { get; private set; } = 10;

        /// <summary>
        /// 메인 Graphics 객체
        /// </summary>
        private Graphics mCanvasDC { get; set; } = null;

        /// <summary>
        /// 임시 Graphics 객체
        /// </summary>
        private Graphics tCanvasDC { get; set; } = null;

        /// <summary>
        /// 비트맵 정의
        /// </summary>
        private Bitmap mtCanvasBitmap = null;

        /// <summary>
        /// 보드 배열
        /// </summary>
        private byte[,] mBackBoard = null;
        /*
         * 보드 설정 값 정의
         * ABCD EEEE(2진수로 각 자리마다 의미 있음
         * A: 셀 오픈 여부
         * B: 깃발 설정 여부
         * C: ?설정 여부
         * D: 지뢰 설정 여부
         * EEEE: 해당 셀에서의 지뢰 개수
         */

        private byte[,] mBackUpBoard = null;

        /// <summary>
        /// 랜덤 객체 정의
        /// </summary>
        private Random mRandom = new Random((int)DateTime.Now.Ticks);

        private int mConformCount = 0;
        private int mFlagCount = 0;
        private int mQuestionCount = 0;
        private string mPlayState = string.Empty;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="width">그리드 가로 크기</param>
        /// <param name="height">그리드 세로 크기</param>
        /// <param name="bombcount">지뢰 개수</param>
        public MineSweeperWorld()
        {
            InitBoard(10,10,10);
        }

        /// <summary>
        /// 보드 초기화
        /// </summary>
        public void InitBoard(int width, int height, int bombcount)
        {
            GridWidth = width;

            GridHeight = height;

            BombCount = bombcount;

            mConformCount = 0;

            mFlagCount = 0;

            mQuestionCount = 0;

            mPlayState = string.Empty;

            mBackBoard = new byte[GridWidth, GridHeight];
            mBackUpBoard = new byte[GridWidth, GridHeight];

            SetBomb();

            SetBoardInit();
        }

        /// <summary>
        /// 지뢰 생성
        /// </summary>
        private void SetBomb()
        {
            int cnt = 0;

            do {
                int x = mRandom.Next(0, GridWidth - 1);
                int y = mRandom.Next(0, GridHeight - 1);

                if ((mBackBoard[x, y] & 0x10) == 0) {
                    mBackBoard[x, y] |= 0x10;
                    cnt++;
                }

            } while (cnt < BombCount);
        }

        /// <summary>
        /// 보드 초기 설정
        /// </summary>
        private void SetBoardInit()
        {
            for (int i = 0; i < GridWidth; i++) {
                for (int j = 0; j < GridHeight; j++) {
                    mBackBoard[i, j] |= 0x80;

                    mBackBoard[i, j] |= (byte)CalcBombCount(i, j);
                }
            }
        }

        /// <summary>
        /// 지뢰 개수 계산
        /// </summary>
        /// <param name="x">그리드 X좌표</param>
        /// <param name="y">그리드 Y좌표</param>
        /// <returns>지뢰 개수</returns>
        private int CalcBombCount(int x, int y)
        {
            int retvalue = 0;
            int m = ((x - 1) < 0 ? 0 : (x - 1));
            int n = ((y - 1) < 0 ? 0 : (y - 1));
            int m_max = ((x + 1) < GridWidth ? (x + 1) : GridWidth - 1);
            int n_max = ((y + 1) < GridHeight ? (y + 1) : GridHeight - 1);

            for(int i = m ;i <= m_max ; i++) {
                for (int j = n; j <= n_max; j++) {
                    if((mBackBoard[i, j] & 0x10) > 0) {
                        retvalue++;
                    }
                }
            }

            return retvalue;
        }

        /// <summary>
        /// Graphics 객체 설정
        /// </summary>
        /// <param name="g">메인 Graphics 객체</param>
        public void SetGraphics(Graphics g)
        {
            // 메인 Graphics 객체 설정
            if (mCanvasDC != null) {
                mCanvasDC.Dispose();
                mCanvasDC = null;
            }

            mCanvasDC = g;

            // 보드를 그리기 위한 비트맵 객체 생성
            if(mtCanvasBitmap != null) {
                mtCanvasBitmap.Dispose();
                mtCanvasBitmap = null;
            }

            mtCanvasBitmap = new Bitmap((int)g.VisibleClipBounds.Width, (int)g.VisibleClipBounds.Height , mCanvasDC);
            // 비트맵 객체의 Graphics 객체 설정
            if (tCanvasDC != null) {
                tCanvasDC.Dispose();
                tCanvasDC = null;
            }

            tCanvasDC = Graphics.FromImage(mtCanvasBitmap);

            tCanvasDC.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            tCanvasDC.Clear(Color.White);
        }

        /// <summary>
        /// 셀 오픈 
        /// </summary>
        /// <param name="x">X좌표</param>
        /// <param name="y">Y좌표</param>
        /// <param name="isRight">마우스 오른쪽 클릭 여부</param>
        /// <returns></returns>
        public bool SetBoardState(int x, int y, bool isRight = false)
        {
            bool retValue = false;

            if (!isRight) {
                // 마우스 왼쪽 버튼 클릭

                if ((mBackBoard[x, y] & 0x80) > 0 && (mBackBoard[x, y] & 0x60) == 0) {
                    // 오픈한 셀이 아니고 깃발이나 물음표 표시 셀도 아님

                    if ((mBackBoard[x, y] & 0x10) > 0) {
                        // 지뢰인 경우 해당 셀만 오픈
                        mBackBoard[x, y] &= 0x7F;
                    } else {
                        // 지뢰가 아닌 경우 주변 셀 모두 오픈
                        OpenTile(x, y);
                    }
                    
                    retValue = true;
                }
            } else {
                // 마우스 오른쪽 버튼 클릭

                if ((mBackBoard[x, y] & 0x80) > 0) {
                    // 오픈하지 않은 셀
                    byte state = (byte)(mBackBoard[x, y] & 0x60);

                    if (state == 0x00) {
                        // 오픈하지 않은 일반셀
                        mBackBoard[x, y] |= 0x40;
                    } else if (state == 0x40) {
                        // 깃발 표시 셀
                        mBackBoard[x, y] &= 0xBF;
                        mBackBoard[x, y] |= 0x20;
                    } else {
                        // 물음표 셀
                        mBackBoard[x, y] &= 0x9F;
                    }

                    retValue = true;
                }
            }

            return retValue;
        }

        /// <summary>
        /// 현재 셀 주변 8개의 셀 검사 및 그리기
        /// </summary>
        /// <param name="x">X좌표</param>
        /// <param name="y">Y좌표</param>
        /// <returns></returns>
        public bool DrawRect(int x, int y)
        {
            if((mBackBoard[x, y] & 0x80) == 0 && (mBackBoard[x, y] & 0x0F) != 0) {
                // 오픈한 셀이며 주변에 지뢰가 존재하는 셀

                int m = ((x - 1) < 0 ? 0 : (x - 1));
                int n = ((y - 1) < 0 ? 0 : (y - 1));
                int m_max = ((x + 1) < GridWidth ? (x + 1) : GridWidth - 1);
                int n_max = ((y + 1) < GridHeight ? (y + 1) : GridHeight - 1);

                // 확정(깃발표시 지뢰) 카운트
                int conformCount = 0;
                bool allopened = false;

                for (int i = m; i <= m_max; i++) {
                    for (int j = n; j <= n_max; j++) {
                        if((mBackBoard[i, j] & 0x40) > 0 && (mBackBoard[i, j] & 0x10) > 0) {
                            conformCount++;
                        }
                    }
                }

                // 주변 8간에 더이상 남은 지뢰가 없으므로 모든 셀 오픈
                if(conformCount == (mBackBoard[x, y] & 0x0F)) {
                    allopened = true;
                }

                // 셀 눌림 상태 그리기
                for (int i = m; i <= m_max; i++) {
                    for (int j = n; j <= n_max; j++) {
                        if ((mBackBoard[i, j] & 0x80) > 0 && (mBackBoard[i, j] & 0x60) == 0) {
                            int x1 = 0, y1 = 0;
                            x1 = Margin + (i * GridSize);
                            y1 = Margin + (j * GridSize);

                            tCanvasDC.DrawImage(Properties.Resources.Tail, x1, y1, GridSize, GridSize);

                            if (allopened) {
                                mBackBoard[i, j] &= 0x7F;
                            } else {
                                mBackUpBoard[i, j] &= 0x7F;
                            }
                        }
                    }
                }

                mCanvasDC.DrawImage(mtCanvasBitmap, 0, 0, mtCanvasBitmap.Width, mtCanvasBitmap.Height);
            }

            return false;
        }

        /// <summary>
        /// 셀 오픈 
        /// </summary>
        /// <param name="x">X좌표</param>
        /// <param name="y">Y좌표</param>
        private void OpenTile(int x, int y)
        {
            byte value = mBackBoard[x, y];
            bool iscovered = (value & 0x80) > 0 ? true : false;
            bool isflag = (value & 0x40) > 0 ? true : false;
            bool isquestion = (value & 0x20) > 0 ? true : false;
            bool isbomb = (value & 0x10) > 0 ? true : false;
            int bombcount = (byte)(value & 0x0F);

            if (iscovered && !isbomb) {
                // 오픈하지 않은 셀이고 지뢰가 아닌 경우
                if(!isflag && !isquestion) {
                    // 깃발 표시 셀이 아니고 물음표 표시 셀도 아님

                    // 셀 오픈
                    mBackBoard[x, y] &= 0x7F;
                }

                // 주변에 지뢰가 없는 셀이면 종료
                if(bombcount != 0) {
                    return;
                }

                int m = ((x - 1) < 0 ? 0 : (x - 1));
                int n = ((y - 1) < 0 ? 0 : (y - 1));
                int m_max = ((x + 1) < GridWidth ? (x + 1) : GridWidth - 1);
                int n_max = ((y + 1) < GridHeight ? (y + 1) : GridHeight - 1);

                // 주변 셀을 재귀적으로 검사하여 모든 공백 셀 오픈
                for (int i = m; i <= m_max; i++) {
                    for (int j = n; j <= n_max; j++) {
                        OpenTile(i, j);
                    }
                }
            }
        }

        /// <summary>
        /// 보드 그리기
        /// </summary>
        /// <param name="aBackColor">초기화 색상</param>
        private void DrawBackBoard(Color aBackColor)
        {
            // Graphics 초기화
            //tCanvasDC.Clear(aBackColor);

            // 확정 셀 카운트
            mConformCount = 0;
            // 깃발 카운트
            mFlagCount = 0;
            // 물음표 카운트
            mQuestionCount = 0;
            // 플레이 상태
            mPlayState = string.Empty;

            for (int i = 0; i < GridWidth; i++) {
                for (int j = 0; j < GridHeight; j++) {
                    if(mBackBoard [i, j] != mBackUpBoard[i, j]) {
                        int x = 0, y = 0;
                        x = Margin + (i * GridSize);
                        y = Margin + (j * GridSize);

                        if((mBackBoard[i, j] & 0x80) > 0 ) {
                            // 현재 열지 않은 셀이면

                            byte state = (byte)(mBackBoard[i, j] & 0x60);
                            byte bomb = (byte)(mBackBoard[i, j] & 0x10);

                            if (state == 0x40) {
                                // 깃발 표시 셀

                                tCanvasDC.DrawImage(Properties.Resources.Flag, x, y, GridSize, GridSize);

                                // 지뢰 셀이랑 일치하면 확정 셀 카운트 증가
                                if (bomb > 0) {
                                    mConformCount++;
                                }

                                mFlagCount++;
                            } else if (state == 0x20) {
                                // 물음표 표시 셀

                                tCanvasDC.DrawImage(Properties.Resources.Question, x, y, GridSize, GridSize);

                                mQuestionCount++;
                            } else {
                                // 오픈하지 않은 일반 셀
                                tCanvasDC.DrawImage(Properties.Resources.Rectangle, x, y, GridSize, GridSize);
                            }
                        } else {
                            // 오픈한 셀

                            byte state = (byte)(mBackBoard[i, j] & 0x10);

                            if(state == 0) {
                                // 지뢰 셀이 아님
                                tCanvasDC.DrawImage(Properties.Resources.Tail, x, y, GridSize, GridSize);

                                byte bombcnt = (byte)(mBackBoard[i, j] & 0x0F);

                                // 셀 주변의 지뢰 숫자 그리기
                                if(bombcnt > 0) {
                                    DrawBombCount(BombCountColors[bombcnt - 1], bombcnt.ToString(), i, j);
                                }
                            } else {
                                // 지뢰 셀임
                                tCanvasDC.DrawImage(Properties.Resources.Bomb, x, y, GridSize, GridSize);
                                mPlayState = "DEFEAT";
                            }
                        }
                    }
                }
            }

            Array.Copy(mBackBoard, mBackUpBoard, mBackBoard.Length);

            // 승리판정(깃발 개수와 폭탄개수 확정 셀 개수가 일치
            if (mFlagCount == BombCount && mConformCount == BombCount) {
                mPlayState = "WIN";
            }
        }

        /// <summary>
        /// 지뢰 개수 문자열 그리기
        /// </summary>
        /// <param name="aColor">문자 색상</param>
        /// <param name="aValue">지뢰 개수</param>
        /// <param name="aX">X 좌표</param>
        /// <param name="aY">Y 좌표</param>
        private void DrawBombCount(Brush aColor, string aValue, int aX, int aY)
        {
            // 폰트 설정
            Font font = new Font("맑은 고딕", 12);

            // 문자 정렬 설정
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // 문자 사이즈 계산
            int strWsize = (int)Math.Round(tCanvasDC.MeasureString(aValue, font).Width);
            int strHsize = (int)Math.Round(tCanvasDC.MeasureString(aValue, font).Height);

            // 문자 그리는 좌표 계산
            int x = Margin + (GridSize * aX) + ((GridSize - strWsize) / 2);
            int y = Margin + (GridSize * aY) + ((GridSize - strHsize) / 2);

            // 문자 그리기
            Rectangle aRect = new Rectangle(x, y, strWsize, strHsize);
            tCanvasDC.DrawString(aValue, font, aColor, aRect, stringFormat);
        }

        /// <summary>
        /// 보드 그리기
        /// </summary>
        public void DrawBoard()
        {
            // 보드 그리기
            DrawBackBoard(Color.White);

            // 메인 Graphics 객체에 그리기
            mCanvasDC.DrawImage(mtCanvasBitmap, 0, 0, mtCanvasBitmap.Width, mtCanvasBitmap.Height);

            // 플레이 상태 정보 이벤트 생성
            DoOnPlayData(mPlayState, mFlagCount, mQuestionCount);
        }

        /// <summary>
        /// 로그 전달 이벤트 설정
        /// </summary>
        /// <param name="message">로그 메세지</param>
        private void DoOnTraceOut(string message)
        {
            OnTraceOut?.Invoke(null, new TraceOutEventArgs(message));
        }

        /// <summary>
        /// 로그 기록
        /// </summary>
        /// <param name="aMessage">로그 메세지</param>
        private void TraceOut(string aMessage)
        {
            DoOnTraceOut(aMessage);
        }

        /// <summary>
        /// 로그기록
        /// </summary>
        /// <param name="aFromat">문자열 포멧</param>
        /// <param name="aArgs">문자열 파라메터</param>
        private void TraceOut(string aFromat, params object[] aArgs)
        {
            TraceOut(string.Format(aFromat, aArgs));
        }

        /// <summary>
        /// 플레이 정보 전달 이벤트 설정
        /// </summary>
        /// <param name="playstate">플레이 상태</param>
        /// <param name="flagcount">깃발 개수</param>
        /// <param name="questioncount">물음표 개수</param>
        private void DoOnPlayData(string playstate, int flagcount, int questioncount)
        {
            OnPlayData?.Invoke(null, new PlayDataEventArgs(playstate, flagcount, questioncount));
        }
    }
}
