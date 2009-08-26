﻿using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.core
{
    public class ContourPickup
    {
        //巡回参照できるように、テーブルを二重化
        //                                           0  1  2  3  4  5  6  7   0  1  2  3  4  5  6
        protected static int[] _getContour_xdir = { 0, 1, 1, 1, 0, -1, -1, -1, 0, 1, 1, 1, 0, -1, -1 };
        protected static int[] _getContour_ydir = { -1, -1, 0, 1, 1, 1, 0, -1, -1, -1, 0, 1, 1, 1, 0 };

        /**
         * ラスタのエントリポイントから辿れる輪郭線を配列に返します。
         * @param i_raster
         * @param i_entry_x
         * @param i_entry_y
         * @param i_array_size
         * @param o_coord_x
         * @param o_coord_y
         * @return
         * 輪郭線の長さを返します。
         * @throws NyARException
         */
        public int getContour(NyARBinRaster i_raster, int i_entry_x, int i_entry_y, int i_array_size, int[] o_coord_x, int[] o_coord_y)
	{
		int[] xdir = _getContour_xdir;// static int xdir[8] = { 0, 1, 1, 1, 0,-1,-1,-1};
		int[] ydir = _getContour_ydir;// static int ydir[8] = {-1,-1, 0, 1, 1, 1, 0,-1};

		int[] i_buf=(int[])i_raster.getBufferReader().getBuffer();
		int width=i_raster.getWidth();
		int height=i_raster.getHeight();
		//クリップ領域の上端に接しているポイントを得る。


		int coord_num = 1;
		o_coord_x[0] = i_entry_x;
		o_coord_y[0] = i_entry_y;
		int dir = 5;

		int c = i_entry_x;
		int r = i_entry_y;
		for (;;) {
			dir = (dir + 5) % 8;//dirの正規化
			//ここは頑張ればもっと最適化できると思うよ。
			//4隅以外の境界接地の場合に、境界チェックを省略するとかね。
			if(c>=1 && c<width-1 && r>=1 && r<height-1){
				for(;;){//gotoのエミュレート用のfor文
					//境界に接していないとき
					if (i_buf[(r + ydir[dir])*width+(c + xdir[dir])] == 0) {
						break;
					}
					dir++;
					if (i_buf[(r + ydir[dir])*width+(c + xdir[dir])] == 0) {
						break;
					}
					dir++;
					if (i_buf[(r + ydir[dir])*width+(c + xdir[dir])] == 0) {
						break;
					}
					dir++;
					if (i_buf[(r + ydir[dir])*width+(c + xdir[dir])] == 0) {
						break;
					}
					dir++;
					if (i_buf[(r + ydir[dir])*width+(c + xdir[dir])] == 0) {
						break;
					}
					dir++;
					if (i_buf[(r + ydir[dir])*width+(c + xdir[dir])] == 0) {
						break;
					}
					dir++;
					if (i_buf[(r + ydir[dir])*width+(c + xdir[dir])] == 0) {
						break;
					}
					dir++;
					if (i_buf[(r + ydir[dir])*width+(c + xdir[dir])] == 0) {
						break;
					}
/*
					try{
						BufferedImage b=new BufferedImage(width,height,ColorSpace.TYPE_RGB);
						NyARRasterImageIO.copy(i_raster, b);
					ImageIO.write(b,"png",new File("bug.png"));
					}catch(Exception e){
						
					}*/
					//8方向全て調べたけどラベルが無いよ？
					throw new NyARException();			
				}
			}else{
				//境界に接しているとき				
				int i;
				for (i = 0; i < 8; i++){				
					int x=c + xdir[dir];
					int y=r + ydir[dir];
					//境界チェック
					if(x>=0 && x<width && y>=0 && y<height){
						if (i_buf[(y)*width+(x)] == 0) {
							break;
						}
					}
					dir++;//倍長テーブルを参照するので問題なし
				}
				if (i == 8) {
					//8方向全て調べたけどラベルが無いよ？
					throw new NyARException();// return(-1);
				}				
			}
			
			dir=dir% 8;//dirの正規化

			// xcoordとycoordをc,rにも保存
			c = c + xdir[dir];
			r = r + ydir[dir];
			o_coord_x[coord_num] = c;
			o_coord_y[coord_num] = r;
			// 終了条件判定
			if (c == i_entry_x && r == i_entry_y){
				coord_num++;
				break;
			}
			coord_num++;
			if (coord_num == i_array_size) {
				//輪郭が末端に達した
				return coord_num;
			}
		}
		return coord_num;
	}
        public int getContour(NyARLabelingImage i_raster, int i_entry_x, int i_entry_y, int i_array_size, int[] o_coord_x, int[] o_coord_y)
        {
            int[] xdir = _getContour_xdir;// static int xdir[8] = { 0, 1, 1, 1, 0,-1,-1,-1};
            int[] ydir = _getContour_ydir;// static int ydir[8] = {-1,-1, 0, 1, 1, 1, 0,-1};

            int[] i_buf = (int[])i_raster.getBufferReader().getBuffer();
            int width = i_raster.getWidth();
            int height = i_raster.getHeight();
            //クリップ領域の上端に接しているポイントを得る。
            int sx = i_entry_x;
            int sy = i_entry_y;

            int coord_num = 1;
            o_coord_x[0] = sx;
            o_coord_y[0] = sy;
            int dir = 5;

            int c = o_coord_x[0];
            int r = o_coord_y[0];
            for (; ; )
            {
                dir = (dir + 5) % 8;//dirの正規化
                //ここは頑張ればもっと最適化できると思うよ。
                //4隅以外の境界接地の場合に、境界チェックを省略するとかね。
                if (c >= 1 && c < width - 1 && r >= 1 && r < height - 1)
                {
                    for (; ; )
                    {//gotoのエミュレート用のfor文
                        //境界に接していないとき
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        dir++;
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        dir++;
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        dir++;
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        dir++;
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        dir++;
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        dir++;
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        dir++;
                        if (i_buf[(r + ydir[dir]) * width + (c + xdir[dir])] > 0)
                        {
                            break;
                        }
                        //8方向全て調べたけどラベルが無いよ？
                        throw new NyARException();
                    }
                }
                else
                {
                    //境界に接しているとき
                    int i;
                    for (i = 0; i < 8; i++)
                    {
                        int x = c + xdir[dir];
                        int y = r + ydir[dir];
                        //境界チェック
                        if (x >= 0 && x < width && y >= 0 && y < height)
                        {
                            if (i_buf[(y) * width + (x)] > 0)
                            {
                                break;
                            }
                        }
                        dir++;//倍長テーブルを参照するので問題なし
                    }
                    if (i == 8)
                    {
                        //8方向全て調べたけどラベルが無いよ？
                        throw new NyARException();// return(-1);
                    }
                }

                dir = dir % 8;//dirの正規化

                // xcoordとycoordをc,rにも保存
                c = c + xdir[dir];
                r = r + ydir[dir];
                o_coord_x[coord_num] = c;
                o_coord_y[coord_num] = r;
                // 終了条件判定
                if (c == sx && r == sy)
                {
                    coord_num++;
                    break;
                }
                coord_num++;
                if (coord_num == i_array_size)
                {
                    //輪郭が末端に達した
                    return coord_num;
                }
            }
            return coord_num;
        }
    }
}