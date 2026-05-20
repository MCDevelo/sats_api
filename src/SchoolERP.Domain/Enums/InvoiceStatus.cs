namespace SchoolERP.Domain.Enums;

public enum InvoiceStatus { Pending, Partial, Paid, Overdue, Cancelled }

public enum PaymentMethod { Cash, Card, Transfer, Cheque, OnlineAzul, OnlineCardnet, Other }

public enum ContractType { Permanent, Temporary, Substitute, PartTime, Contractor }

public enum EducationLevel { Inicial, Primaria, Secundaria }

public enum Shift { Matutino, Vespertino, Nocturno }

public enum Gender { M, F }

public enum DocumentStatus { Pending, Received, Verified, Rejected }
