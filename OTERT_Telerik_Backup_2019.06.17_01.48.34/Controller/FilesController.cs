using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OTERT.Model;
using OTERT_Entity;

namespace OTERT.Controller {

    public class FilesController {

        public int CountFiles(int taskID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    return dbContext.Files.Where(k => k.TaskID == taskID).Count();
                }
                catch (Exception) { return -1; }
            }
        }

        public List<FileB> GetFilesByTaskID(int taskID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<FileB> data = (from us in dbContext.Files
                                              select new FileB {
                                                    ID = us.ID,
                                                    OrderID = us.OrderID,
                                                    TaskID = us.TaskID,
                                                    FilePath = us.FilePath,
                                                    FileName = us.FileName,
                                                    DateStamp = us.DateStamp
                                              }).Where(k => k.TaskID == taskID).OrderBy(o => o.DateStamp).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<FileB> GetFilesByTaskID(int taskID, int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<FileB> data = (from us in dbContext.Files
                                        select new FileB {
                                            ID = us.ID,
                                            OrderID = us.OrderID,
                                            TaskID = us.TaskID,
                                            FilePath = us.FilePath,
                                            FileName = us.FileName,
                                            DateStamp = us.DateStamp
                                        }).Where(k => k.TaskID == taskID).OrderBy(o => o.DateStamp).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<FileB> GetFilesByOrderID(int orderID) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<FileB> data = (from us in dbContext.Files
                                        select new FileB {
                                            ID = us.ID,
                                            OrderID = us.OrderID,
                                            TaskID = us.TaskID,
                                            FilePath = us.FilePath,
                                            FileName = us.FileName,
                                            DateStamp = us.DateStamp
                                        }).Where(k => k.OrderID == orderID).OrderBy(o => o.DateStamp).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

        public List<FileB> GetFilesByOrderID(int orderID, int recSkip, int recTake) {
            using (var dbContext = new OTERTConnStr()) {
                try {
                    dbContext.Configuration.ProxyCreationEnabled = false;
                    List<FileB> data = (from us in dbContext.Files
                                        select new FileB {
                                            ID = us.ID,
                                            OrderID = us.OrderID,
                                            TaskID = us.TaskID,
                                            FilePath = us.FilePath,
                                            FileName = us.FileName,
                                            DateStamp = us.DateStamp
                                        }).Where(k => k.OrderID == orderID).OrderBy(o => o.DateStamp).Skip(recSkip).Take(recTake).ToList();
                    return data;
                }
                catch (Exception) { return null; }
            }
        }

    }

}