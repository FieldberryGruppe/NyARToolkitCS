﻿using System;
using System.Collections.Generic;
using System.Text;

namespace jp.nyatla.nyartoolkit.cs.nyidmarker
{
    public class NyIdMarkerDataEncoder_RawBit : INyIdMarkerDataEncoder
    {
        private const int _DOMAIN_ID = 0;
        private int[] _mod_data = { 7, 31, 127, 511, 2047, 4095 };
        public bool encode(NyIdMarkerPattern i_data, INyIdMarkerData o_dest)
        {
            NyIdMarkerData_RawBit dest = (NyIdMarkerData_RawBit)o_dest;
            if (i_data.ctrl_domain != _DOMAIN_ID)
            {
                return false;
            }
            //パケット数計算
            int resolution_len = (i_data.model + 1);
            int packet_length = (resolution_len * resolution_len) / 8 + 1;
            int sum = 0;
            for (int i = 0; i < packet_length; i++)
            {
                dest.packet[i] = i_data.data[i];
                sum += i_data.data[i];
            }
            //チェックドット値計算
            sum = sum % _mod_data[i_data.model - 2];
            //チェックドット比較
            if (i_data.check != sum)
            {
                return false;
            }
            dest.length = packet_length;
            return true;
        }
        public INyIdMarkerData createDataInstance()
        {
            return new NyIdMarkerData_RawBit();
        }
    }
}
