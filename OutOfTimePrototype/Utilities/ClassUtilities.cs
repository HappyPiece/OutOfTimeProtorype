using OutOfTimePrototype.DAL.Models;
using System.Net;

namespace OutOfTimePrototype.Utilities
{
    public static class ClassUtilities
    {
        public class ClassOperationResult
        {
            public ClassOperationStatus? Status { get; set; }
            public string? Message { get; set; }
            public List<Class>? QueryResult { get; set; }

            public HttpStatusCode HttpStatusCode { get; set; }
            public ClassOperationResult(ClassOperationStatus classOperationStatus, HttpStatusCode httpStatusCode, 
                string? errorMessage = null, List<Class>? queryResult = null)
            {
                Status = classOperationStatus;
                Message = errorMessage;
                HttpStatusCode = httpStatusCode;
                QueryResult = queryResult;
            }
            

            public enum ClassOperationStatus
            {
                Success,
                ClassCreated,
                ClassEdited,
                ClassDeleted,
                TimeSlotNotFound,
                ClusterNotFound,
                ClusterOccupied,
                LectureHallNotFound,
                LectureHallOccupied,
                EducatorNotFound,
                EducatorOccupied,
                ClassTypeNotFound,
                ClassNotFound,
                UnspecifiedDate,
                UnspecifiedCluster,
                UnspecifiedTimeSlot
            }

            /// <summary>
            /// Creates default ClassOperationResult based on what status was supplied as first parameter.
            /// Second parameter is evaluated according to the value of the first and is used to create describeful message.
            /// </summary>
            /// <param name="status"></param>
            /// <param name="arg"></param>
            /// <returns></returns>
            public static ClassOperationResult GenerateDefaultClassOperationResult(ClassOperationStatus status, string? arg = null, List<Class>? queryResult = null)
            {
                string message = "";
                HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
                switch (status)
                {
                    case ClassOperationStatus.Success:
                        {
                            message = "Operation successful";
                            httpStatusCode = HttpStatusCode.OK;
                            break;
                        }
                    case ClassOperationStatus.ClassCreated:
                        {
                            message = $"Class was successfully created, Id {arg}";
                            httpStatusCode = HttpStatusCode.Created;
                            break;
                        }
                    case ClassOperationStatus.ClassEdited:
                        {
                            message = $"Class with id {arg} was successfully edited";
                            httpStatusCode = HttpStatusCode.OK;
                            break;
                        }
                    case ClassOperationStatus.ClassDeleted:
                        {
                            message = $"Class with id {arg} was successfully deleted";
                            httpStatusCode = HttpStatusCode.OK;
                            break;
                        }
                    case ClassOperationStatus.TimeSlotNotFound:
                        {
                            message = $"Time slot number '{arg}' does not exist";
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;
                        }
                    case ClassOperationStatus.ClusterNotFound:
                        {
                            message = $"Cluster number '{arg}' does not exist";
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;
                        }
                    case ClassOperationStatus.ClusterOccupied:
                        {
                            message = $"Cluster number '{arg}' is occupied at specified time";
                            httpStatusCode = HttpStatusCode.Conflict;
                            break;
                        }
                    case ClassOperationStatus.LectureHallNotFound:
                        {
                            message = $"Lecture hall with id '{arg}' does not exist";
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;
                        }
                    case ClassOperationStatus.LectureHallOccupied:
                        {
                            message = $"Lecture hall with id '{arg}' is occupied at specified time";
                            httpStatusCode = HttpStatusCode.Conflict;
                            break;
                        }
                    case ClassOperationStatus.EducatorNotFound:
                        {
                            message = $"Educator with id '{arg}' does not exist";
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;
                        }
                    case ClassOperationStatus.EducatorOccupied:
                        {
                            message = $"Educator with id '{arg}' is occupied at specified time";
                            httpStatusCode = HttpStatusCode.Conflict;
                            break;
                        }
                    case ClassOperationStatus.ClassTypeNotFound:
                        {
                            message = $"Class type with name '{arg}' does not exist";
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;
                        }
                    case ClassOperationStatus.ClassNotFound:
                        {
                            message = $"Class with id '{arg}' does not exist";
                            httpStatusCode = HttpStatusCode.NotFound;
                            break;
                        }
                    case ClassOperationStatus.UnspecifiedDate:
                        {
                            message = $"Date property was not supplied";
                            httpStatusCode = HttpStatusCode.BadRequest;
                            break;
                        }
                    case ClassOperationStatus.UnspecifiedCluster:
                        {
                            message = $"Cluster property was not supplied";
                            httpStatusCode = HttpStatusCode.BadRequest;
                            break;
                        }
                    case ClassOperationStatus.UnspecifiedTimeSlot:
                        {
                            message = $"TimeSlot property was not supplied";
                            httpStatusCode = HttpStatusCode.BadRequest;
                            break;
                        }
                }
                return new ClassOperationResult(status, httpStatusCode, errorMessage: message, queryResult: queryResult);
            }
        }
    }
}
