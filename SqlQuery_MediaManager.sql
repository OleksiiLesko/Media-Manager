CREATE DATABASE MediaManager;
GO

USE MediaManager;
GO

CREATE TABLE CallDirection(
    CallDirectionId SMALLINT  PRIMARY KEY NOT NULL,
    CallDirectionType VARCHAR(50)
);
GO

INSERT INTO CallDirection (CallDirectionId, CallDirectionType)
VALUES
    (1, 'Unknown'),
    (2, 'Incoming'),
    (3, 'Outcoming'),
    (4, 'Internal');

CREATE TABLE MediaType(
    MediaTypeId SMALLINT  PRIMARY KEY NOT NULL,
    MediaTypeName VARCHAR(50) 
);
GO

INSERT INTO MediaType (MediaTypeId, MediaTypeName)
VALUES
    (1, 'Voice'),
    (2, 'Screen');

CREATE TABLE RecordingStatus(
    RecordingStatusId SMALLINT  PRIMARY KEY NOT NULL,
    RecordingStatusType VARCHAR(50) 
);
GO

INSERT INTO RecordingStatus (RecordingStatusId, RecordingStatusType)
VALUES
    (1, 'None'),
    (2, 'Recorded'),
    (3, 'NotRecorded');

CREATE TABLE ArchivingStatus(
    ArchivingStatusId SMALLINT  PRIMARY KEY NOT NULL,
    ArchivingStatusType VARCHAR(50) 
);
GO

INSERT INTO ArchivingStatus (ArchivingStatusId, ArchivingStatusType)
VALUES
    (1, 'GoingToArchive'),
    (2, 'Archived'),
    (3, 'FailedToArchive'),
    (4, 'Deleted');

CREATE TABLE CallEvent(
    CallId INT  PRIMARY KEY NOT NULL,
    CallStartTime DateTime,
    CallEndTime DateTime,
    CallDirection SMALLINT,
  CONSTRAINT FK_CallEvent_CallDirection FOREIGN KEY (CallDirection) REFERENCES CallDirection(CallDirectionId)
);
GO

CREATE TABLE Recording(
    CallId INT  NOT NULL,
    RecordingId INT PRIMARY KEY NOT NULL,
    RecorderId INT NOT NULL,
    StartTime DateTime,
    EndTime DateTime,
    MediaType SMALLINT,
    RecordingStatus SMALLINT,
    ArchivingFilePath VARCHAR(200),
    ArchivingDate DateTime,
    ArchivingStatus SMALLINT,
  CONSTRAINT FK_Recording_ArchivingStatus FOREIGN KEY (ArchivingStatus) REFERENCES ArchivingStatus(ArchivingStatusId),
  CONSTRAINT FK_Recording_MediaType FOREIGN KEY (MediaType) REFERENCES MediaType(MediaTypeId),
  CONSTRAINT FK_Recording_RecordingStatus FOREIGN KEY (RecordingStatus) REFERENCES RecordingStatus(RecordingStatusId),
  CONSTRAINT FK_Recording_CallEvent FOREIGN KEY (CallId) REFERENCES CallEvent(CallId) ON DELETE NO ACTION
);
GO