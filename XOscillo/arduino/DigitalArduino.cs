﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XOscillo
{
   
   class DigitalArduino : SerialArduino
   {
      int m_numSamples = 1024;
      int SampleID = 0;

      public DigitalArduino()
         : base(115200, 12000)
      {
         //needs tunning
         //baudrate = 1000000;
         //m_sampleRate = 59250; // this is actual number of samples per second I am able to archieve on the arduino

         m_triggerValue = 1;
      }

      DateTime time = new DateTime();
      byte[] res = new byte[1];

      public void Config()
      {
         byte[] configBuffer = new byte[4];
         configBuffer[0] = (byte)COMMANDS.READ_BIN_TRACE; ;
         configBuffer[1] = m_triggerValue;
         configBuffer[2] = (byte)(m_numSamples >> 8);
         configBuffer[3] = (byte)(m_numSamples & 0xff);

         serialPort.DiscardInBuffer();
         serialPort.Write(configBuffer, 0, configBuffer.Length);
      }

      override public bool GetDataBlock(ref DataBlock db)
      {
         bool result;

         Config();

         time = DateTime.Now;

         Read(res, 1);
         if (res[0] == 85)
         {
            db.m_sample = SampleID++;
            db.m_start = DateTime.Now;

            db.m_channels = 6;
            db.m_trigger = 0;
            db.m_sampleRate = m_sampleRate;
            db.m_stride = 1;
            db.m_channelOffset = 0;
            db.m_dataType = DataBlock.DATA_TYPE.DIGITAL;

            if (db.m_Buffer == null || db.m_Buffer.Length != m_numSamples)
            {
               db.Alloc(m_numSamples);
            }

            result = Read(db.m_Buffer, m_numSamples);
            db.m_stop = DateTime.Now;
            return result;
         }

         return false;
      }


   }
    
}
