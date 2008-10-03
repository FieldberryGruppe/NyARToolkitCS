﻿/* 
 * PROJECT: NyARToolkitCS
 * --------------------------------------------------------------------------------
 * This work is based on the original ARToolKit developed by
 *   Hirokazu Kato
 *   Mark Billinghurst
 *   HITLab, University of Washington, Seattle
 * http://www.hitl.washington.edu/artoolkit/
 *
 * The NyARToolkit is Java version ARToolkit class library.
 * Copyright (C)2008 R.Iizuka
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this framework; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * For further information please contact.
 *	http://nyatla.jp/nyatoolkit/
 *	<airmail(at)ebony.plala.or.jp>
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using jp.nyatla.nyartoolkit.cs.core;

namespace jp.nyatla.nyartoolkit.cs.sandbox.x2
{
    /**
     * This class calculates ARMatrix from square information and holds it. --
     * 変換行列を計算して、結果を保持するクラス。
     * 
     */
    public class NyARTransMat_X2 : NyARTransMat
    {
        private NyARSinTable _sin_table = new NyARSinTable(1024);
        public NyARTransMat_X2(NyARParam i_param):base()
        {
            NyARCameraDistortionFactor dist = i_param.getDistortionFactor();
            NyARPerspectiveProjectionMatrix pmat = i_param.getPerspectiveProjectionMatrix();
            this._calculator = new NyARFitVecCalculator(pmat, dist);
            this._rotmatrix = new NyARRotMatrix_X2(pmat, this._sin_table);
            this._mat_optimize = new NyARRotTransOptimize_X2(pmat, this._sin_table);
            return;
        }
    }
}
