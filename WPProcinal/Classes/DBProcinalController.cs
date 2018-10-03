﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WPProcinal.ADO;
using WPProcinal.Models;

namespace WPProcinal.Classes
{
    public class DBProcinalController
    {
        private static DBProcinalEntities db = new DBProcinalEntities();

        public static Response SaveDipMap(DipMap dipMap)
        {
            try
            {
                var tblDipMap = new TblDipMap
                {
                    Active = true,
                    Category = dipMap.Category,
                    CinemaId = dipMap.CinemaId,
                    Date = dipMap.Date,
                    DateFormat = dipMap.DateFormat,
                    Day = dipMap.Day,
                    Duration = dipMap.Duration,
                    Gener = dipMap.Gener,
                    Group = dipMap.Group,
                    Hour = dipMap.Hour,
                    HourFormat = dipMap.HourFormat,
                    HourFunction = dipMap.HourFunction,
                    IsCard = dipMap.IsCard,
                    Language = dipMap.Language,
                    Letter = dipMap.Letter,
                    Login = dipMap.Login,
                    MovieId = dipMap.MovieId,
                    MovieName = dipMap.MovieName,
                    PointOfSale = dipMap.PointOfSale,
                    RoomId = dipMap.RoomId,
                    RoomName = dipMap.RoomName,
                    Secuence = dipMap.Secuence
                };

                db.TblDipMaps.Add(tblDipMap);
                db.SaveChanges();
                return new Response
                {
                    IsSuccess = true,
                    Result = tblDipMap.DipMapId,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        internal static List<TblTypeSeat> GetSeatsreverve()
        {
            try
            {
                var seats = db.TblTypeSeats.Where(s => s.IsReserved == true).ToList();
                return seats;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Response SaveTypeSeats(List<TypeSeat> tyseats,int dipMap)
        {
            try
            {
                foreach (var tyseat in tyseats)
                {
                    var seat = new TblTypeSeat
                    {
                        CodTarifa = tyseat.CodTarifa,
                        DipMapId = dipMap,
                        IsReserved = tyseat.IsReserved,
                        Letter = tyseat.Letter,
                        Name = tyseat.Name,
                        Number = tyseat.Number,
                        NumSecuencia = tyseat.NumSecuencia,
                        Price = tyseat.Price,
                        Type = tyseat.Type,
                        IsPay = false,
                    };

                    db.TblTypeSeats.Add(seat);
                    db.SaveChanges();
                }

                return new Response
                {
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public static Response EditSeatDesAssingReserve(int dipMapId)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var dipmap = db.TblDipMaps.Find(dipMapId);
                    dipmap.Active = false;
                    db.Entry(dipmap).State = EntityState.Modified;
                    db.SaveChanges();
                    var seats = db.TblTypeSeats.Where(s => s.DipMapId == dipMapId).ToList();
                    foreach (var seat in seats)
                    {
                        seat.IsReserved = false;
                        seat.IsPay = false;
                        db.Entry(seat).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    transaction.Commit();
                    return new Response
                    {
                        IsSuccess = true
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new Response
                    {
                        IsSuccess = false,
                        Message = ex.Message,
                    };
                } 
            }
        }

        public static Response EditPaySeat(int dipMapId)
        {
            try
            {
                var seats = db.TblTypeSeats.Where(s => s.DipMapId == dipMapId).ToList();
                foreach (var seat in seats)
                {
                    seat.IsPay = true;
                    db.Entry(seat).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return new Response
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public static List<TblDipMap> GetDipMapActive()
        {
            try
            {
                var dipmap = db.TblDipMaps.ToList();
                return dipmap;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<TblTypeSeat> GetSeats(int id)
        {
            try
            {
                var seats = db.TblTypeSeats.Where(s => s.DipMapId == id).ToList();
                return seats;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool UpdateDipMap(int id)
        {
            try
            {
                var dipmap = db.TblDipMaps.Where(d => d.DipMapId == id).FirstOrDefault();
                if (dipmap == null)
                {
                    return false;
                }

                dipmap.Active = false;
                db.Entry(dipmap).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static TblDipMap GetDipMap2(int id)
        {
            try
            {
                var dipmap = db.TblDipMaps.Where(d => d.DipMapId == id).FirstOrDefault();
                if (dipmap == null)
                {
                    return null;
                }

                return dipmap;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<TblPay> GetDipmapNotPay()
        {
            try
            {
                var notPays = db.TblPays.Where(t => t.IsPay == true).ToList();
                return notPays;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static Response SaveDipmapNotPay(int id)
        {
            try
            {
                var notPay = new TblPay
                {
                    DipMapId = id,
                    IsPay = true,
                };

                db.TblPays.Add(notPay);
                db.SaveChanges();
                return new Response
                {
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public static Response UpdateDipmapNotPay(int id)
        {
            try
            {
                var tblPay = db.TblPays.Where(t => t.DipMapId == id).FirstOrDefault();
                if (tblPay == null)
                {
                    return new Response
                    {
                        IsSuccess = true,
                    };
                }
                
                db.TblPays.Remove(tblPay);
                db.SaveChanges();
                return new Response
                {
                    IsSuccess = true,
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public static Response RemoveDipmap(int id)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    var dipmap = db.TblDipMaps.Find(id);
                    if (dipmap == null)
                    {
                        transaction.Rollback();
                        return new Response
                        {
                            IsSuccess = true,
                        };
                    }

                    var seats = db.TblTypeSeats.Where(s => s.DipMapId == id).ToList();
                    foreach (var item in seats)
                    {
                        db.TblTypeSeats.Remove(item);
                        db.SaveChanges();
                    }

                    db.TblDipMaps.Remove(dipmap);
                    db.SaveChanges();
                    transaction.Commit();
                    return new Response
                    {
                        IsSuccess = true,
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();                    
                    return new Response
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    };
                } 
            }
        }
    }
}
