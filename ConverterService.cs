﻿/*******************************************************************************
	RIP2Image is a program that efficiently converts formats such as PDF or Postscript to image formats such as Jpeg or PNG.
    Copyright (C) 2013 XMPie Ltd.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

	This license covers only the RIP2Image files and not any file that RIP2Image links against or otherwise uses.
 
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*******************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Threading;
// using log4net;
// using log4net.Config;

/*[assembly: log4net.Config.XmlConfigurator(Watch = true)]*/

namespace RIP2Jmage
{
	/// <summary>
	/// Uniting all convert utilities.
	/// </summary>
	class ConverterService : IConverterService
	{
		//private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConverterService()
		{
		}


		#region Methods
		public bool ConvertPDF2JPG(string inConvertFilePath, string inNewFileTargetFolderPath, double inResolutionX, double inResolutionY, 
									double inGraphicsAlphaBitsValue, double inTextAlphaBitsValue, double inQuality)
		{
// 			logger.Info("inConvertFilePath = " + inConvertFilePath + ", inNewFileTargetFolderPath = " + inNewFileTargetFolderPath +
// 						", inResolutionX = " + inResolutionX + ", inResolutionY = " + inResolutionY + ", inGraphicsAlphaBitsValue = " + inGraphicsAlphaBitsValue +
// 						", inTextAlphaBitsValue = " + inTextAlphaBitsValue + ", inQuality = " + inQuality);

			bool conversionSucceed;

			CheckParamValidation(inResolutionX, inResolutionY, inGraphicsAlphaBitsValue, inTextAlphaBitsValue, inQuality);

			// Make the conversion.
			FileConverter fileConvertor = InstancesManager.GetObject(InstancesManager.ConversionType.PDF2JPG);
			conversionSucceed = fileConvertor.ConvertPDF2JPG(inConvertFilePath, inNewFileTargetFolderPath, inResolutionX, inResolutionY, inGraphicsAlphaBitsValue, inTextAlphaBitsValue, inQuality);
			InstancesManager.PutObject(InstancesManager.ConversionType.PDF2JPG, fileConvertor);

			// Rename JPG names to the correct page counter.
			RenameJPGNames(inNewFileTargetFolderPath, inConvertFilePath);


			return conversionSucceed;
		}

		public bool ConvertPDFFolder2JPG(string inConvertFolderPath, string inTargetFolderPath, string inConvertFileWildCard, bool inDeleteSourcePDF,
										  bool inSearchSubFolders, double inResolutionX, double inResolutionY, double inGraphicsAlphaBitsValue,
										  double inTextAlphaBitsValue, double inQuality)
		{
// 			logger.Info("inConvertFolderPath = " + inConvertFolderPath + ", inTargetFolderPath = " + inTargetFolderPath +
// 						", inConvertFileWildCard = " + inConvertFileWildCard + ", inDeleteSourcePDF = " + inDeleteSourcePDF + ", inSearchSubFolders = " + inSearchSubFolders +
// 						", inResolutionX = " + inResolutionX + ", inResolutionY = " + inResolutionY + ", inGraphicsAlphaBitsValue = " + inGraphicsAlphaBitsValue +
// 						 ", inTextAlphaBitsValue = " + inTextAlphaBitsValue + ", inQuality = " + inQuality);

			bool conversionSucceed;

			CheckParamValidation(inResolutionX, inResolutionY, inGraphicsAlphaBitsValue, inTextAlphaBitsValue, inQuality);

			System.IO.DirectoryInfo root = new System.IO.DirectoryInfo(inConvertFolderPath);

			// Convert all files in folder.
			FileConverter fileConvertor = InstancesManager.GetObject(InstancesManager.ConversionType.PDF2JPG);
			conversionSucceed = WalkDirectoryTreePDF2JPG(fileConvertor, root, inTargetFolderPath, inConvertFileWildCard, inDeleteSourcePDF, inSearchSubFolders, inConvertFolderPath.Equals(inTargetFolderPath), inResolutionX, inResolutionY, inGraphicsAlphaBitsValue, inTextAlphaBitsValue, inQuality);
			InstancesManager.PutObject(InstancesManager.ConversionType.PDF2JPG, fileConvertor);

			return conversionSucceed;
		}

		public bool ConvertPDF2EPS(string inConvertFilePath, string inNewFileTargetPath, double inFirstPageToConvert, double inLastPageToConvert)
		{
// 			logger.Info("inConvertFilePath = " + inConvertFilePath + ", inNewFileTargetPath = " + inNewFileTargetPath +
// 						", inFirstPageToConvert = " + inFirstPageToConvert + ", inLastPageToConvert = " + inLastPageToConvert);

			bool conversionSucceed;

			// Make the conversion.
			FileConverter fileConvertor = InstancesManager.GetObject(InstancesManager.ConversionType.PDF2EPS);
			conversionSucceed = fileConvertor.ConvertPDF2EPS(inConvertFilePath, inNewFileTargetPath, inFirstPageToConvert, inLastPageToConvert);
			InstancesManager.PutObject(InstancesManager.ConversionType.PDF2EPS, fileConvertor);

			return conversionSucceed;
		}

		public bool ConvertPDFFolder2EPS(string inConvertFolderPath, string inTargetFolderPath, string inConvertFileWildCard, bool inDeleteSourcePDF,
																			bool inSearchSubFolders, double inFirstPageToConvert, double inLastPageToConvert)
		{
// 			logger.Info("inConvertFolderPath = " + inConvertFolderPath + ", inTargetFolderPath = " + inTargetFolderPath +
// 						", inConvertFileWildCard = " + inConvertFileWildCard + ", inDeleteSourcePDF = " + inDeleteSourcePDF + ", inSearchSubFolders = " + inSearchSubFolders +
// 						", inFirstPageToConvert = " + inFirstPageToConvert + ", inLastPageToConvert = " + inLastPageToConvert);

			bool conversionSucceed;

			System.IO.DirectoryInfo root = new System.IO.DirectoryInfo(inConvertFolderPath);

			// Convert all files in folder.
			FileConverter fileConvertor = InstancesManager.GetObject(InstancesManager.ConversionType.PDF2EPS);
			conversionSucceed = WalkDirectoryTreePDF2EPS(fileConvertor, root, inTargetFolderPath, inConvertFileWildCard, inDeleteSourcePDF, inSearchSubFolders, 
														inConvertFolderPath.Equals(inTargetFolderPath), inFirstPageToConvert, inLastPageToConvert);
			InstancesManager.PutObject(InstancesManager.ConversionType.PDF2JPG, fileConvertor);

			return conversionSucceed;
		}

		#endregion

		#region Help Method

		/// <summary>
		/// Check parameters validation.
		/// </summary>
		/// <param name="inResolutionX"></param>
		/// <param name="inResolutionY"></param>
		/// <param name="inGraphicsAlphaBitsValue"></param>
		/// <param name="inTextAlphaBitsValue"></param>
		/// <param name="inQuality"></param>
		/// <returns></returns>
		private void CheckParamValidation(double inResolutionX, double inResolutionY, double inGraphicsAlphaBitsValue, double inTextAlphaBitsValue, double inQuality)
		{
			if (inResolutionX <= 0 || inResolutionY <= 0)
			{
				throw new ArgumentException("Resolution cannot be <= 0");
			}
			else if (!(inGraphicsAlphaBitsValue == 1 || inGraphicsAlphaBitsValue == 2 || inGraphicsAlphaBitsValue == 4))
			{
				throw new ArgumentException("GraphicsAlphaBits values are 1, 2 or 4");
			}
			else if (!(inTextAlphaBitsValue == 1 || inTextAlphaBitsValue == 2 || inTextAlphaBitsValue == 4))
			{
				throw new ArgumentException("TextAlphaBits values are 1, 2 or 4");
			}
			else if (inQuality < 0 || inQuality > 100)
			{
				throw new ArgumentException("File quality range is 0-100");
			}
		}

		/// <summary>
		/// Walking traverse all folders under inRoot looking for PDF files need to convert to JPG.
		/// </summary>
		/// <param name="inFileConvertor"></param>
		/// <param name="inRoot"></param>
		/// <param name="inTargetFolderPath"></param>
		/// <param name="inConvertFileWildCard"></param>
		/// <param name="inDeleteSourcePDF"></param>
		/// <param name="inSearchSubFolders"> If true traverse each sub-folders and convert them, except if one of the sub-folders is the target folder. </param>
		/// <param name="inSameTargetFolder"> If false create new sub folder under target folder path with the same name as the root sub-folder. </param>
		/// <param name="inResolutionX"></param>
		/// <param name="inResolutionY"></param>
		private bool WalkDirectoryTreePDF2JPG(FileConverter inFileConvertor, System.IO.DirectoryInfo inRoot, string inTargetFolderPath, string inConvertFileWildCard,
												bool inDeleteSourcePDF, bool inSearchSubFolders, bool inSameTargetFolder, double inResolutionX, double inResolutionY, 
												double inGraphicsAlphaBitsValue, double inTextAlphaBitsValue, double inQuality)
		{
			bool fileConversion;

			System.IO.FileInfo[] files = null;
			System.IO.DirectoryInfo[] subDirs = null;

			// First, process all the files directly under this folder
			files = inRoot.GetFiles(inConvertFileWildCard);
			//TODO: add logs

			if (files != null)
			{
				foreach (System.IO.FileInfo file in files)
				{
					// Make file conversion.
					fileConversion = inFileConvertor.ConvertPDF2JPG(file.FullName, inTargetFolderPath, inResolutionX, inResolutionY, inGraphicsAlphaBitsValue, inTextAlphaBitsValue, inQuality);
					if (!fileConversion)
						return false;

					//Delete old files.
					if (inDeleteSourcePDF)
						FileDelete(file.FullName);


					// Rename JPG names to the correct page counter.
					RenameJPGNames(inTargetFolderPath, file.FullName);
				}

				if (inSearchSubFolders)
				{
					// Now find all the subdirectories under this directory.
					subDirs = inRoot.GetDirectories();
					foreach (System.IO.DirectoryInfo dirInfo in subDirs)
					{
						// In case the target folder is sub directory of the converted folder don't check it. 
						if (inTargetFolderPath.Contains(dirInfo.FullName))
							continue;
						if (!inSameTargetFolder)
						{
							//Create a new sub folder under target folder path
							string newPath = System.IO.Path.Combine(inTargetFolderPath, dirInfo.Name);
							//Create the sub folder
							System.IO.Directory.CreateDirectory(newPath);
							//Recursive call for each subdirectory.
							WalkDirectoryTreePDF2JPG(inFileConvertor, dirInfo, newPath, inConvertFileWildCard, inDeleteSourcePDF, inSearchSubFolders, inSameTargetFolder, inResolutionX, inResolutionY, inGraphicsAlphaBitsValue, inTextAlphaBitsValue, inQuality);
						}
						else
						{
							// Recursive call for each subdirectory.
							WalkDirectoryTreePDF2JPG(inFileConvertor, dirInfo, dirInfo.FullName, inConvertFileWildCard, inDeleteSourcePDF, inSearchSubFolders, inSameTargetFolder, inResolutionX, inResolutionY, inGraphicsAlphaBitsValue, inTextAlphaBitsValue, inQuality);
						}

					}
				}

			}

			return true;
		}

		/// <summary>
		/// Walking traverse all folders under inRoot looking for PDF files need to convert to EPS.
		/// </summary>
		/// <param name="inFileConvertor"></param>
		/// <param name="inRoot"></param>
		/// <param name="inTargetFolderPath"></param>
		/// <param name="inConvertFileWildCard"></param>
		/// <param name="inDeleteSourcePDF"></param>
		/// <param name="inSearchSubFolders"> If true traverse each sub-folders and convert them, except if one of the sub-folders is the target folder. </param>
		/// <param name="inSameTargetFolder"> If false create new sub folder under target folder path with the same name as the root sub-folder. </param>
		/// <param name="inFirstPageToConvert"></param>
		/// <param name="inLastPageToConvert"></param>
		/// <returns></returns>
		private bool WalkDirectoryTreePDF2EPS(FileConverter inFileConvertor, System.IO.DirectoryInfo inRoot, string inTargetFolderPath, string inConvertFileWildCard,
												bool inDeleteSourcePDF, bool inSearchSubFolders, bool inSameTargetFolder, double inFirstPageToConvert, double inLastPageToConvert)
		{
			bool fileConversion;

			System.IO.FileInfo[] files = null;
			System.IO.DirectoryInfo[] subDirs = null;

			// First, process all the files directly under this folder
			files = inRoot.GetFiles(inConvertFileWildCard);
			//TODO: add logs

			if (files != null)
			{
				foreach (System.IO.FileInfo file in files)
				{
					// Create converted EPS path.
					string convertedEPSPath = inTargetFolderPath + "\\" + Path.GetFileNameWithoutExtension(file.FullName) + ".eps";

					// Make file conversion.
					fileConversion = inFileConvertor.ConvertPDF2EPS(file.FullName, convertedEPSPath, inFirstPageToConvert, inLastPageToConvert);
					if (!fileConversion)
						return false;

					//Delete old files.
					if (inDeleteSourcePDF)
						FileDelete(file.FullName);
				}

				if (inSearchSubFolders)
				{
					// Now find all the subdirectories under this directory.
					subDirs = inRoot.GetDirectories();
					foreach (System.IO.DirectoryInfo dirInfo in subDirs)
					{
						// In case the target folder is sub directory of the converted folder don't check it. 
						if (inTargetFolderPath.Contains(dirInfo.FullName))
							continue;

						if (!inSameTargetFolder)
						{
							//Create new sub folder under target folder path
							string newPath = System.IO.Path.Combine(inTargetFolderPath, dirInfo.Name);
							//Create the sub folder
							System.IO.Directory.CreateDirectory(newPath);
							//Recursive call for each subdirectory.
							WalkDirectoryTreePDF2EPS(inFileConvertor, dirInfo, newPath, inConvertFileWildCard, inDeleteSourcePDF, inSearchSubFolders, inSameTargetFolder, inFirstPageToConvert, inLastPageToConvert);
						}
						else
						{
							// Recursive call for each subdirectory.
							WalkDirectoryTreePDF2EPS(inFileConvertor, dirInfo, dirInfo.FullName, inConvertFileWildCard, inDeleteSourcePDF, inSearchSubFolders, inSameTargetFolder, inFirstPageToConvert, inLastPageToConvert);
						}

					}
				}

			}

			return true;
		}

		/// <summary>
		/// Rename JPG names to the correct name and page counter.
		/// </summary>
		/// <param name="inFileDir">Target folder path</param>
		/// <param name="inFileFullName">File full path name</param>
		private void RenameJPGNames(string inFileDir, string inFileFullName)
		{
			string[] filesNameWithTheSamePrefix = Directory.GetFiles(inFileDir, Path.GetFileNameWithoutExtension(inFileFullName) + "*");

			int filesCounter = 1;
			foreach (string fileName in filesNameWithTheSamePrefix)
			{
				if (fileName.EndsWith(".jpg"))
				{
					string pageNumberOutputFormat = GeneratePageNumberOutputFormat(filesCounter);
					string fileNewName = inFileDir + "\\" + Path.GetFileNameWithoutExtension(inFileFullName) + pageNumberOutputFormat + filesCounter + ".jpg";
					// Rename file.
					FileMove(fileName, fileNewName);
					filesCounter++;
				}
			}
		}

		/// <summary>
		/// Generate page number prefix format. 
		/// </summary>
		/// <param name="inFilesCounter"></param>
		/// <returns></returns>
		private string GeneratePageNumberOutputFormat(int inFilesCounter)
		{
			if (inFilesCounter >= 1 && inFilesCounter <= 9)
				return "_p00";
			else if (inFilesCounter >= 10 && inFilesCounter <= 99)
				return "_p0";
			else if (inFilesCounter >= 100 && inFilesCounter <= 999)
				return "_p";

			return null;
		}

		/// <summary>
		/// Tries to move (or rename) file several times in order to avoid unavailable/locked files issues
		/// </summary>		
		private void FileMove(string sourceFileName, string destFileName)
		{
			// try to move file
			int i = 0;
			while (true)
			{
				try
				{
					File.Move(sourceFileName, destFileName);
					break;
				}
				catch (Exception ex)
				{
					if (i < 3)
						Thread.Sleep(++i * 100);
					else
						throw ex;
				}
			}
		}

		/// <summary>
		/// Tries to delete the file several times in order to avoid unavailable/locked files issues
		/// </summary>		
		private void FileDelete(string filePath)
		{
			int i = 0;
			while (true)
			{
				try
				{
					File.Delete(filePath);
					break;
				}
				catch (Exception ex)
				{
					if (i < 3)
						Thread.Sleep(++i * 100);
					else
						throw ex;
				}
			}
		}
		#endregion

	}
}
