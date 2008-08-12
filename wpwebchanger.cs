// Copyright (c) 2004 Michael Zehrer <zehrer@zepan.org>
// All rights reserved.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Library General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.

using System;
using System.Net;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WpWebChanger {
        
        class ChangeWpFromWeb {
                
                private static string requestedFile;
                
                [STAThread]
                static void Main(string[] args) {
                        if (args.Length < 1) {
                                Console.WriteLine("Usage: wpchanger.exe <image url>");
                        } else {
                                FileStream fstr = null;
                                Stream str = null;
                                try {
                                        string downloadUrl = args.GetValue(0).ToString();
                                        
                                        HttpWebRequest downloadRequest = (HttpWebRequest) WebRequest.Create(downloadUrl);
                                        downloadRequest.Timeout=6000;
                                        HttpWebResponse downloadResponse = (HttpWebResponse) downloadRequest.GetResponse();
                                        
                                        str = downloadResponse.GetResponseStream();
                                        byte[] inBuf = new byte[downloadResponse.ContentLength];
                                        int bytesToRead = (int) inBuf.Length;
                                        int bytesRead = 0;
                                        while (bytesToRead > 0) {
                                                int n = str.Read(inBuf, bytesRead,bytesToRead);
                                                if (n==0)
                                                        break;
                                                bytesRead += n;
                                                bytesToRead -= n;
                                        }
                                        
                                        int iStartPos = downloadUrl.LastIndexOf("/") + 1;
                                        requestedFile = downloadUrl.Substring(iStartPos);
                                        
                                        fstr = new FileStream(System.Environment.SystemDirectory + "/" + requestedFile, FileMode.OpenOrCreate,FileAccess.Write);
                                        fstr.Write(inBuf, 0, bytesRead);
                                        str.Close();
                                        fstr.Close();
                                        changeit();
                                } catch(Exception e) {
                                        Console.WriteLine("Error while downloading: " + e);
                                }
                        }
                }
                
                private static void changeit() {
                        
                        string wpfile = System.Environment.SystemDirectory + "/wpwebchanger.bmp";
                        try {
                                Image imgInFile=Image.FromFile(System.Environment.SystemDirectory + "/" + requestedFile);
                                imgInFile.Save(wpfile,ImageFormat.Bmp);
                                
                                int retVal = WinAPI.SystemParametersInfo(20, 0, wpfile, 0x1|0x2);
                        } catch(Exception e) {
                                Console.WriteLine("Conversion error: " + e);
                        }
                }
        }
        
        public class WinAPI {
                
                [DllImport("user32.dll", CharSet=CharSet.Auto)]
                public static  extern int SystemParametersInfo (int
                                                                uAction , int uParam , string lpvParam , int fuWinIni) ;
        }
        
}
