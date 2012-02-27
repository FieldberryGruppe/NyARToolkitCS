using UnityEngine;
using System;
using System.Collections;
using NyARUnityUtils;
using jp.nyatla.nyartoolkit.cs.markersystem;
using jp.nyatla.nyartoolkit.cs.core;

namespace NyARUnityUtils
{
	public class NyARUnityMarkerSystem:NyARMarkerSystem
	{
		public NyARUnityMarkerSystem(INyARMarkerSystemConfig i_config):base(i_config)
		{
		}
		protected override void initInstance(INyARMarkerSystemConfig i_config)
		{
			base.initInstance(i_config);		
			this._projection_mat=new Matrix4x4();
		}
	
		private Matrix4x4 _projection_mat;
	
		/**
		 * OpenGLスタイルのProjectionMatrixを返します。
		 * @param i_gl
		 * @return
		 * [readonly]
		 */
		public Matrix4x4 getUnityProjectionMatrix()
		{
			return this._projection_mat;
		}
		public override void setProjectionMatrixClipping(double i_near,double i_far)
		{
			base.setProjectionMatrixClipping(i_near,i_far);
			NyARUnityUtil.toCameraFrustumRH(this._ref_param,1,i_near,i_far,ref this._projection_mat);
			
		}
		private Matrix4x4 _work=new Matrix4x4();
		/**
		 * 
		 * この関数はOpenGL形式の姿勢変換行列を返します。
		 * 返却値の有効期間は、次回の{@link #getGlMarkerTransMat()}をコールするまでです。
		 * 値を保持する場合は、{@link #getGlMarkerMatrix(double[])}を使用します。
		 * @param i_buf
		 * @return
		 * [readonly]
		 */
		public Matrix4x4 getUnityMarkerMatrix(int i_id)
		{
			return this.getUnityMarkerMatrix(i_id,ref this._work);
		}
		/**
		 * この関数は、i_bufに指定idのOpenGL形式の姿勢変換行列を設定して返します。
		 * @param i_id
		 * @param i_buf
		 * @return
		 */
		public Matrix4x4 getUnityMarkerMatrix(int i_id,ref Matrix4x4 i_buf)
		{
			NyARUnityUtil.toCameraViewRH(this.getMarkerMatrix(i_id),1,ref i_buf);
			return i_buf;
		}
		public void setTransformFromMatrix(int i_id, Transform trans)
		{
			Matrix4x4 m=this.getUnityMarkerMatrix(i_id);
    		trans.rotation =transformRotMatToQuaternion(ref m);
    		trans.position =m.GetColumn(3);
		}
		/// <summary>
		/// http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
		/// </summary>
		/// <returns>
		/// The rot mat to quaternion.
		/// </returns>
		/// <param name='i_mat'>
		/// I_mat.
		/// </param>
		private Quaternion transformRotMatToQuaternion(ref Matrix4x4 i_mat)
		{
			float tr = i_mat.m00 + i_mat.m11 + i_mat.m22;
			Quaternion q = new Quaternion();
			if (tr > 0){
			  float S = (float)Math.Sqrt(tr+1.0) * 2; // S=4*qw 
			  q.w =( 0.25f * S);
			  q.x =((i_mat.m21 - i_mat.m12) / S);
			  q.y =((i_mat.m02 - i_mat.m20) / S); 
			  q.z =((i_mat.m10 - i_mat.m01) / S); 
			} else if ((i_mat.m00 > i_mat.m11)&(i_mat.m00 > i_mat.m22)) { 
			  float S = (float)Math.Sqrt(1.0f + i_mat.m00 - i_mat.m11 - i_mat.m22) * 2; // S=4*qx 
			  q.w =( (i_mat.m21 - i_mat.m12) / S);
			  q.x =( 0.25f * S);
			  q.y =( (i_mat.m01 + i_mat.m10) / S); 
			  q.z =( (i_mat.m02 + i_mat.m20) / S); 
			} else if (i_mat.m11 > i_mat.m22) { 
			  float S = (float)Math.Sqrt(1.0f + i_mat.m11 - i_mat.m00 - i_mat.m22) * 2; // S=4*qy
			  q.w =( (i_mat.m02 - i_mat.m20) / S);
			  q.x =( (i_mat.m01 + i_mat.m10) / S); 
			  q.y =( 0.25f * S);
			  q.z =( (i_mat.m12 + i_mat.m21) / S); 
			} else { 
			  float S = (float)Math.Sqrt(1.0 + i_mat.m22 - i_mat.m00 - i_mat.m11) * 2; // S=4*qz
			  q.w =((i_mat.m10 - i_mat.m01) / S);
			  q.x =((i_mat.m02 + i_mat.m20) / S);
			  q.y =((i_mat.m12 + i_mat.m21) / S);
			  q.z =(0.25f * S);
			}
		    return q;
		}
	}
}

