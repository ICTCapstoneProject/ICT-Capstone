PRAGMA foreign_keys=OFF;
BEGIN TRANSACTION;
CREATE TABLE groups (
    group_id INTEGER PRIMARY KEY,
    name VARCHAR NOT NULL UNIQUE
);
CREATE TABLE roles (
    role_id INTEGER PRIMARY KEY,
    role_name VARCHAR NOT NULL UNIQUE CHECK(length(role_name) > 0)
);
INSERT INTO roles VALUES(1,'Researcher');
INSERT INTO roles VALUES(2,'Manager');
INSERT INTO roles VALUES(3,'Assistant Director');
INSERT INTO roles VALUES(4,'Admin');
CREATE TABLE user_groups (
    user_id INTEGER,
    group_id INTEGER,
    role_id INTEGER,
    PRIMARY KEY (user_id, group_id),
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE,
    FOREIGN KEY (group_id) REFERENCES groups(group_id) ON DELETE CASCADE,
    FOREIGN KEY (role_id) REFERENCES roles(role_id)
);
CREATE TABLE notifications (
    notification_id INTEGER PRIMARY KEY,
    user_id INTEGER NOT NULL,
    message TEXT NOT NULL,
    is_read BOOLEAN NOT NULL DEFAULT FALSE CHECK (is_read IN (0, 1)),
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE
);
CREATE TABLE status (
    status_id INTEGER PRIMARY KEY,
    status_name VARCHAR NOT NULL UNIQUE
);
INSERT INTO status VALUES(1,'New Proposal');
INSERT INTO status VALUES(2,'In Progress');
INSERT INTO status VALUES(3,'Complete');
CREATE TABLE reviews (
    review_id INTEGER PRIMARY KEY,
    proposal_id INTEGER NOT NULL,
    reviewed_by INTEGER NOT NULL,
    decision VARCHAR CHECK (decision IN ('Approved', 'Rejected', 'Needs Changes')),
    timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (proposal_id) REFERENCES "proposals"(id),
    FOREIGN KEY (reviewed_by) REFERENCES users(user_id)
);
CREATE TABLE comments (
    comment_id INTEGER PRIMARY KEY,
    review_id INTEGER NOT NULL,
    commenter INTEGER NOT NULL,
    comment TEXT NOT NULL,
    timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (review_id) REFERENCES reviews(review_id),
    FOREIGN KEY (commenter) REFERENCES users(user_id)
);
CREATE TABLE ethics_review (
    ethics_id INTEGER PRIMARY KEY,
    proposal_id INTEGER NOT NULL,
    ethics_required BOOLEAN NOT NULL CHECK (ethics_required IN (0, 1)),
    hrec_name VARCHAR,
    hrec_number VARCHAR,
    opinion VARCHAR CHECK (opinion IN ('Approved', 'Rejected', 'Pending')),
    reviewed_by INTEGER,
    review_date DATE,
    FOREIGN KEY (proposal_id) REFERENCES "proposals"(id),
    FOREIGN KEY (reviewed_by) REFERENCES users(user_id)
);
CREATE TABLE IF NOT EXISTS "project_levels" (
    level_id INT PRIMARY KEY,
    level_name NVARCHAR(255)
);
INSERT INTO project_levels VALUES(1,'Honours');
INSERT INTO project_levels VALUES(2,'PhD');
INSERT INTO project_levels VALUES(3,'Vacation Studentship');
INSERT INTO project_levels VALUES(4,'Internal');
INSERT INTO project_levels VALUES(5,'Industry');
INSERT INTO project_levels VALUES(6,'Other');
CREATE TABLE IF NOT EXISTS "__EFMigrationsLock" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK___EFMigrationsLock" PRIMARY KEY,
    "Timestamp" TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);
INSERT INTO __EFMigrationsHistory VALUES('20250515031523_InitClean','9.0.5');
CREATE TABLE proposal_log (
    log_id INTEGER PRIMARY KEY AUTOINCREMENT,
    proposal_id INTEGER NOT NULL,
    status_id INTEGER NOT NULL,
    changed_by INTEGER NOT NULL,
    timestamp DATETIME DEFAULT CURRENT_TIMESTAMP NOT NULL,
    action TEXT NOT NULL DEFAULT 'submitted',
    FOREIGN KEY (proposal_id) REFERENCES "proposals"(id),
    FOREIGN KEY (status_id) REFERENCES status(status_id), 
    FOREIGN KEY (changed_by) REFERENCES users(user_id)
);
INSERT INTO proposal_log VALUES(1,33,1,1,'2025-05-15 12:56:33.227476','submitted');
INSERT INTO proposal_log VALUES(2,1,1,1,'2025-04-15 09:15:15.021027','submitted');
INSERT INTO proposal_log VALUES(3,2,1,1,'2025-04-15 09:36:59.268297','submitted');
INSERT INTO proposal_log VALUES(4,3,1,1,'2025-04-15 09:41:17.66349','submitted');
INSERT INTO proposal_log VALUES(5,4,1,1,'2025-04-15 09:47:51.930495','submitted');
INSERT INTO proposal_log VALUES(6,5,1,1,'2025-04-15 09:51:54.475499','submitted');
INSERT INTO proposal_log VALUES(7,6,1,1,'2025-04-15 09:54:59.522252','submitted');
INSERT INTO proposal_log VALUES(8,7,1,1,'2025-04-15 10:01:09.921613','submitted');
INSERT INTO proposal_log VALUES(9,8,1,1,'2025-04-15 10:02:20.883165','submitted');
INSERT INTO proposal_log VALUES(10,9,1,1,'2025-04-15 10:02:56.345821','submitted');
INSERT INTO proposal_log VALUES(11,10,1,1,'2025-04-15 10:06:11.505041','submitted');
INSERT INTO proposal_log VALUES(12,11,1,1,'2025-04-15 10:51:13.579091','submitted');
INSERT INTO proposal_log VALUES(13,12,1,1,'2025-04-15 10:53:40.129928','submitted');
INSERT INTO proposal_log VALUES(14,13,1,1,'2025-04-15 10:57:21.135','submitted');
INSERT INTO proposal_log VALUES(15,14,1,1,'2025-04-15 11:00:02.936525','submitted');
INSERT INTO proposal_log VALUES(16,15,1,1,'2025-04-15 11:01:37.919093','submitted');
INSERT INTO proposal_log VALUES(17,16,1,1,'2025-04-15 11:22:40.791846','submitted');
INSERT INTO proposal_log VALUES(18,17,1,1,'2025-04-15 11:26:45.601936','submitted');
INSERT INTO proposal_log VALUES(19,18,1,1,'2025-04-18 16:59:56.881902','submitted');
INSERT INTO proposal_log VALUES(20,19,1,1,'2025-04-20 01:26:31.905735','submitted');
INSERT INTO proposal_log VALUES(21,20,1,2,'2025-04-20 22:31:55.472481','submitted');
INSERT INTO proposal_log VALUES(22,21,1,1,'2025-04-30 20:15:32.926706','submitted');
INSERT INTO proposal_log VALUES(23,22,1,1,'2025-04-30 21:23:43.25659','submitted');
INSERT INTO proposal_log VALUES(24,23,1,1,'2025-04-30 22:06:52.710322','submitted');
INSERT INTO proposal_log VALUES(25,24,1,2,'2025-04-30 23:01:30.564369','submitted');
INSERT INTO proposal_log VALUES(26,25,1,2,'2025-04-30 23:29:23.474271','submitted');
INSERT INTO proposal_log VALUES(27,26,1,1,'2025-05-01 13:14:08.128815','submitted');
INSERT INTO proposal_log VALUES(28,27,1,1,'2025-05-15 11:14:39.647142','submitted');
INSERT INTO proposal_log VALUES(29,28,1,1,'2025-05-15 12:35:38.963999','submitted');
INSERT INTO proposal_log VALUES(30,29,1,1,'2025-05-15 12:42:16.210811','submitted');
INSERT INTO proposal_log VALUES(31,30,1,1,'2025-05-15 12:44:00.624707','submitted');
INSERT INTO proposal_log VALUES(32,31,1,1,'2025-05-15 12:47:22.931291','submitted');
INSERT INTO proposal_log VALUES(33,32,1,1,'2025-05-15 12:52:49.992277','submitted');
INSERT INTO proposal_log VALUES(35,1,1,1,'2025-05-15 13:08:16.966163','modified');
INSERT INTO proposal_log VALUES(36,24,1,1,'2025-05-15 14:06:24.623716','modified');
INSERT INTO proposal_log VALUES(37,1,1,1,'2025-05-15 14:22:01.317017','modified');
INSERT INTO proposal_log VALUES(38,32,1,1,'2025-05-15 14:54:00.018994','modified');
INSERT INTO proposal_log VALUES(39,1,1,1,'2025-05-15 15:37:03.298676','modified');
INSERT INTO proposal_log VALUES(40,1,1,1,'2025-05-15 15:39:04.773445','modified');
INSERT INTO proposal_log VALUES(41,1,1,1,'2025-05-15 15:40:25.725347','modified');
INSERT INTO proposal_log VALUES(42,34,1,1,'2025-05-15 22:28:02.472828','submitted');
INSERT INTO proposal_log VALUES(43,35,1,1,'2025-05-15 22:37:37.810215','submitted');
INSERT INTO proposal_log VALUES(44,36,1,1,'2025-05-15 22:43:38.06777','submitted');
INSERT INTO proposal_log VALUES(45,26,1,1,'2025-05-16 01:48:48.781625','modified');
INSERT INTO proposal_log VALUES(46,37,1,1,'2025-05-16 02:43:50.122148','submitted');
INSERT INTO proposal_log VALUES(47,38,1,1,'2025-05-16 02:50:10.535266','submitted');
INSERT INTO proposal_log VALUES(48,32,1,1,'2025-05-16 03:01:10.513758','modified');
INSERT INTO proposal_log VALUES(49,32,1,1,'2025-05-16 03:01:38.101801','modified');
INSERT INTO proposal_log VALUES(50,39,1,1,'2025-05-16 08:49:30.979588','submitted');
INSERT INTO proposal_log VALUES(51,39,1,1,'2025-05-16 08:50:48.757645','modified');
INSERT INTO proposal_log VALUES(52,26,1,1,'2025-05-18 15:50:31.997647','modified');
CREATE TABLE user_roles (
    user_id INTEGER NOT NULL,
    role_id INTEGER NOT NULL,
    PRIMARY KEY (user_id, role_id),
    FOREIGN KEY (user_id) REFERENCES users(user_id),
    FOREIGN KEY (role_id) REFERENCES roles(role_id)
);
INSERT INTO user_roles VALUES(2,2);
INSERT INTO user_roles VALUES(3,3);
INSERT INTO user_roles VALUES(4,4);
INSERT INTO user_roles VALUES(1,2);
INSERT INTO user_roles VALUES(5,1);
INSERT INTO user_roles VALUES(6,1);
CREATE TABLE IF NOT EXISTS "users" (
	"user_id"	INTEGER,
	"name"	VARCHAR NOT NULL,
	"email"	VARCHAR NOT NULL UNIQUE,
	"password_hash"	TEXT NOT NULL,
	"created_at"	DATETIME DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY("user_id")
);
INSERT INTO users VALUES(1,'Guest User','guest@example.com','mockhash','2025-04-14 23:18:13');
INSERT INTO users VALUES(2,'User2','user2@example.com','password123','2025-04-20 12:59:25');
INSERT INTO users VALUES(3,'Director Dude','AssistantDirector@example.com','director','2025-05-14 23:08:30');
INSERT INTO users VALUES(4,'Admin','Admin@example.com','admin','2025-05-23 06:39:08');
INSERT INTO users VALUES(5,'researcher1','researcher1@example.com','researcher1','2025-05-23 22:55:28.153149');
INSERT INTO users VALUES(6,'Guest2','Guest2@example.com','pass','2025-05-29 01:17:36.92509');
CREATE TABLE attachment_types (
    type_id INTEGER PRIMARY KEY,
    type_name TEXT NOT NULL
);
INSERT INTO attachment_types VALUES(1,'Synopsis Attachment');
INSERT INTO attachment_types VALUES(2,'Method Attachment');
INSERT INTO attachment_types VALUES(3,'Ethics Attachment');
CREATE TABLE attachments (
    file_id INTEGER PRIMARY KEY,
    proposal_id INTEGER NOT NULL,
    file_name TEXT NOT NULL,
    file_path TEXT NOT NULL,
    type_id INTEGER NOT NULL,

    FOREIGN KEY (proposal_id) REFERENCES "proposals"(id),
    FOREIGN KEY (type_id) REFERENCES attachment_types(type_id)
);
CREATE TABLE financial_resources (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    proposal_id INTEGER NOT NULL,
    title TEXT NOT NULL,
    cost REAL NOT NULL CHECK (cost >= 0),
    FOREIGN KEY (proposal_id) REFERENCES "proposals"(id) ON DELETE CASCADE
);
CREATE TABLE proposal_researchers (
    proposal_id INTEGER NOT NULL,
    user_id INTEGER NOT NULL,
    PRIMARY KEY (proposal_id, user_id),
    FOREIGN KEY (proposal_id) REFERENCES "proposals"(id),
    FOREIGN KEY (user_id) REFERENCES users(user_id)
);
CREATE TABLE proposals (
    id INTEGER PRIMARY KEY,
    title TEXT NOT NULL,
    synopsis TEXT NOT NULL,
    method TEXT NOT NULL,
    project_level_id INTEGER NOT NULL,
    physical_resources TEXT NOT NULL,
    ethical_considerations TEXT NOT NULL,
    outcomes TEXT NOT NULL,
    milestones TEXT NOT NULL,
    estimated_completion_date DATETIME NOT NULL,
    status_id INTEGER,
    created_at DATETIME,
    updated_at DATETIME,
    submitted_by INTEGER NOT NULL,
    lead_researcher_id INTEGER
);
INSERT INTO proposals VALUES(1,'Modifying the first proposal AGAIN','blablabla','blablabla',1,'blablabla','blablabla','blablabla','fixed milestones','2025-05-16 00:00:00',1,'2025-04-15 09:15:15.021027','2025-05-15 15:40:25.71284',1,1);
INSERT INTO proposals VALUES(2,'blablabla2','blablabla2','blablabla2',6,'blablabla2','blablabla2','blablabla2','blablabla2','2025-04-17 00:00:00',1,'2025-04-15 09:36:59.268297','2025-04-15 09:36:59.292218',1,1);
INSERT INTO proposals VALUES(3,'blablabla3','blablabla3','blablabla3',5,'blablabla3','blablabla3','blablabla3','blablabla3','2025-04-16 00:00:00',1,'2025-04-15 09:41:17.66349','2025-04-15 09:41:17.687932',1,1);
INSERT INTO proposals VALUES(4,'blablabla4','blablabla4','blablabla4',5,'blablabla4','blablabla4','blablabla4','blablabla4','2025-04-19 00:00:00',1,'2025-04-15 09:47:51.930495','2025-04-15 09:47:51.953792',1,1);
INSERT INTO proposals VALUES(5,'blablabla5','blablabla5','blablabla5',4,'blablabla5','blablabla5','blablabla5','blablabla5','2025-04-16 00:00:00',1,'2025-04-15 09:51:54.475499','2025-04-15 09:51:54.492924',1,1);
INSERT INTO proposals VALUES(6,'blablabla1000','blablabla1000','blablabla1000',4,'blablabla1000','blablabla1000','blablabla1000','blablabla1000','2025-04-17 00:00:00',1,'2025-04-15 09:54:59.522252','2025-04-15 09:54:59.553772',1,1);
INSERT INTO proposals VALUES(7,'blablabla5Billion','blablabla5Billion','blablabla5Billion',6,'blablabla5Billion','blablabla5Billion','blablabla5Billion','blablabla5Billion','2025-04-17 00:00:00',1,'2025-04-15 10:01:09.921613','2025-04-15 10:01:09.943245',1,1);
INSERT INTO proposals VALUES(8,'blablabla5Billion','blablabla5Billion','blablabla5Billion',6,'blablabla5Billion','blablabla5Billion','blablabla5Billion','blablabla5Billion','2025-04-17 00:00:00',1,'2025-04-15 10:02:20.883165','2025-04-15 10:02:20.883186',1,1);
INSERT INTO proposals VALUES(9,'Blablabla9Billion','Blablabla9Billion','Blablabla9Billion',3,'Blablabla9Billion','Blablabla9Billion','Blablabla9Billion','Blablabla9Billion','2025-04-18 00:00:00',1,'2025-04-15 10:02:56.345821','2025-04-15 10:02:56.36632',1,1);
INSERT INTO proposals VALUES(10,'NotRubbish','NotRubbish','NotRubbish',3,'NotRubbish','NotRubbish','NotRubbish','NotRubbish','2025-04-20 00:00:00',1,'2025-04-15 10:06:11.505041','2025-04-15 10:06:11.530666',1,1);
INSERT INTO proposals VALUES(11,'SomeProject','SomeProject','SomeProject',3,'SomeProject','SomeProject','SomeProject','SomeProject','2025-04-17 00:00:00',1,'2025-04-15 10:51:13.579091','2025-04-15 10:51:13.610764',1,1);
INSERT INTO proposals VALUES(12,'blablablabla99','blablablabla99','blablablabla99',5,'blablablabla99','blablablabla99','blablablabla99','blablablabla99','2025-04-19 00:00:00',1,'2025-04-15 10:53:40.129928','2025-04-15 10:53:40.129972',1,1);
INSERT INTO proposals VALUES(13,'blablablebla100000000','blablablebla100000000','blablablebla100000000',3,'blablablebla100000000','blablablebla100000000','blablablebla100000000','blablablebla100000000','2025-04-17 00:00:00',1,'2025-04-15 10:57:21.135','2025-04-15 10:57:21.158551',1,1);
INSERT INTO proposals VALUES(14,'blaba23','blaba23','blaba23',2,'blaba23','blaba23','blaba23','blaba23','2025-04-23 00:00:00',1,'2025-04-15 11:00:02.936525','2025-04-15 11:00:02.955132',1,1);
INSERT INTO proposals VALUES(15,'mmmm','mmmm','mmmm',5,'mmmm','mmmm','mmmm','mmmm','2025-04-25 00:00:00',1,'2025-04-15 11:01:37.919093','2025-04-15 11:01:37.934479',1,1);
INSERT INTO proposals VALUES(16,'adamnd','adamnd','adamnd',2,'adamnd','adamnd','adamnd','adamnd','2025-04-18 00:00:00',1,'2025-04-15 11:22:40.791846','2025-04-15 11:22:40.807988',1,1);
INSERT INTO proposals VALUES(17,'MockProposal','MockSynopsis','MockMethod',2,'MockResources','MockConsiderations','MockOutcomes','MockMilestones','2025-04-25 00:00:00',1,'2025-04-15 11:26:45.601936','2025-04-15 11:26:45.601973',1,1);
INSERT INTO proposals VALUES(18,'title','title','title',6,'title','title','title','title','2025-04-25 00:00:00',1,'2025-04-18 16:59:56.881902','2025-04-18 16:59:56.882009',1,1);
INSERT INTO proposals VALUES(19,'example','example','example',6,'example','example','example','example','2025-05-02 00:00:00',1,'2025-04-20 01:26:31.905735','2025-04-20 01:26:31.905858',1,1);
INSERT INTO proposals VALUES(20,'user2Proposal','user2Proposal','user2Proposal',6,'user2Proposal','user2Proposal','user2Proposal','user2Proposal','2025-04-26 00:00:00',1,'2025-04-20 22:31:55.472481','2025-04-20 22:31:55.472558',2,2);
INSERT INTO proposals VALUES(21,'Making another proposal for guest user','Guest user things','Guest user things',3,'Guest user things','Guest user things','Guest user things','Guest user things','2025-05-02 00:00:00',1,'2025-04-30 20:15:32.926706','2025-04-30 20:15:32.927294',1,1);
INSERT INTO proposals VALUES(22,'Making yet another guest user proposal','Fake stuff','Fake stuff',3,'Fake stuff','Fake stuff','Fake stuff','Fake stuff','2025-05-03 00:00:00',1,'2025-04-30 21:23:43.25659','2025-04-30 21:23:43.256659',1,1);
INSERT INTO proposals VALUES(23,'Making a third proposal for guest user','Fake stuff','Fake stuff',6,'Fake stuff','Fake stuff','Fake stuff','Fake stuff','2025-05-01 00:00:00',1,'2025-04-30 22:06:52.710322','2025-04-30 22:06:52.710325',1,1);
INSERT INTO proposals VALUES(24,'Modifying a User 2 Proposal Title as Guest','Fake','Fake',6,'Fake','Fake','Fake','Fake','2025-05-16 00:00:00',1,'2025-04-30 23:01:30.564369','2025-05-15 14:06:24.599666',2,2);
INSERT INTO proposals VALUES(25,'Making another proposal for User2','Fake','Fake',6,'Fake','Fake','Fake','Fake','2025-05-02 00:00:00',1,'2025-04-30 23:29:23.474271','2025-04-30 23:29:23.474276',2,2);
INSERT INTO proposals VALUES(26,'Editting123','fixed synopsis','Fake',6,'Fake','Fake','Fake','Fake','2025-06-06 00:00:00',1,'2025-05-01 13:14:08.128815','2025-05-18 15:50:31.847377',1,1);
INSERT INTO proposals VALUES(27,'TestingEdit','TestingEdit','TestingEdit',6,'TestingEdit','TestingEdit','TestingEdit','TestingEdit','2025-05-16 00:00:00',1,'2025-05-15 11:14:39.647142','2025-05-15 11:14:39.647337',1,1);
INSERT INTO proposals VALUES(28,'Making a quick proposal','Making a quick proposal','Making a quick proposal',6,'Making a quick proposal','Making a quick proposal','Making a quick proposal','Making a quick proposal','2025-05-15 00:00:00',1,'2025-05-15 12:35:38.963999','2025-05-15 12:35:38.964088',1,1);
INSERT INTO proposals VALUES(29,'Testing log','Testing log','Testing log',6,'Testing log','Testing log','Testing log','Testing log','2025-05-15 00:00:00',1,'2025-05-15 12:42:16.210811','2025-05-15 12:42:16.210893',1,1);
INSERT INTO proposals VALUES(30,'testing log','testing log','testing log',6,'testing log','testing log','testing log','testing log','2025-05-15 00:00:00',1,'2025-05-15 12:44:00.624707','2025-05-15 12:44:00.624779',1,1);
INSERT INTO proposals VALUES(31,'testing log','testing log','testing log',6,'testing log','testing log','testing log','testing log','2025-05-16 00:00:00',1,'2025-05-15 12:47:22.931291','2025-05-15 12:47:22.931451',1,1);
INSERT INTO proposals VALUES(32,'Modifying a dupe title part 2','testing log again','testing log again',3,'testing log again','testing log again','testing log again','testing log again','2025-05-17 00:00:00',1,'2025-05-15 12:52:49.992277','2025-05-16 03:01:38.093295',1,1);
INSERT INTO proposals VALUES(33,'testing log again','testing log again','testing log again',3,'testing log again','testing log again','testing log again','testing log again','2025-05-16 00:00:00',1,'2025-05-15 12:56:33.197084','2025-05-15 12:56:33.19715',1,1);
INSERT INTO proposals VALUES(34,'Testing Image Upload','Testing Image Upload','Testing Image Upload',6,'Testing Image Upload','Testing Image Upload','Testing Image Upload','Testing Image Upload','2025-05-15 00:00:00',1,'2025-05-15 22:28:02.389463','2025-05-15 22:28:02.389601',1,1);
INSERT INTO proposals VALUES(35,'Testing Another Image','Testing Another Image','Testing Another Image',6,'Testing Another Image','Testing Another Image','Testing Another Image','Testing Another Image','2025-05-16 00:00:00',1,'2025-05-15 22:37:37.792851','2025-05-15 22:37:37.792873',1,1);
INSERT INTO proposals VALUES(36,'Testing a Third Image','Testing a Third Image','Testing a Third Image',6,'Testing a Third Image','Testing a Third Image','Testing a Third Image','Testing a Third Image','2025-05-16 00:00:00',1,'2025-05-15 22:43:38.059928','2025-05-15 22:43:38.059959',1,1);
INSERT INTO proposals VALUES(37,'Functionality Testing','Functionality Testing','Functionality Testing',6,'Functionality Testing','Functionality Testing','Functionality Testing','Functionality Testing','2025-05-17 00:00:00',1,'2025-05-16 02:43:49.950895','2025-05-16 02:43:49.950982',1,1);
INSERT INTO proposals VALUES(38,'Final Touchups','Final Touchups','Final Touchups',6,'Final Touchups','Final Touchups','Final Touchups','Final Touchups','2025-05-16 00:00:00',1,'2025-05-16 02:50:10.52555','2025-05-16 02:50:10.525562',1,1);
INSERT INTO proposals VALUES(39,'Quick Proposal','New Synopsis','Quick Proposal',6,'Quick Proposal','Quick Proposal','Quick Proposal','Quick Proposal','2025-05-17 00:00:00',1,'2025-05-16 08:49:30.851127','2025-05-16 08:50:48.746429',1,1);
INSERT INTO proposals VALUES(40,'Testing New Fields','Testing New Fields','Testing New Fields',6,'Testing New Fields','Testing New Fields','Testing New Fields','Testing New Fields','2025-05-30 00:00:00',1,'2025-05-29 10:20:40.167687','2025-05-29 10:20:40.167738',4,0);
INSERT INTO proposals VALUES(41,'Testing New Fields Again','Testing New Fields Again','Testing New Fields Again',3,'Testing New Fields Again','Testing New Fields Again','Testing New Fields Again','Testing New Fields Again','2025-05-30 00:00:00',1,'2025-05-29 10:30:09.841083','2025-05-29 10:30:09.841164',1,0);
CREATE TABLE proposal_history (
    history_id INTEGER PRIMARY KEY AUTOINCREMENT,
    proposal_id INTEGER NOT NULL,
    title TEXT NOT NULL,
    synopsis TEXT NOT NULL,
    method TEXT NOT NULL,
    project_level_id INTEGER NOT NULL,
    physical_resources TEXT NOT NULL,
    ethical_considerations TEXT NOT NULL,
    outcomes TEXT NOT NULL,
    milestones TEXT NOT NULL,
    estimated_completion_date DATETIME NOT NULL,
    lead_researcher_id INTEGER NOT NULL,
    status_id INTEGER NOT NULL,
    action TEXT NOT NULL,
    timestamp DATETIME NOT NULL,
    changed_by INTEGER NOT NULL,
    
    FOREIGN KEY (proposal_id) REFERENCES proposals(id),
    FOREIGN KEY (project_level_id) REFERENCES project_levels(level_id),
    FOREIGN KEY (lead_researcher_id) REFERENCES users(user_id),
    FOREIGN KEY (status_id) REFERENCES statuses(status_id),
    FOREIGN KEY (changed_by) REFERENCES users(user_id)
);
DELETE FROM sqlite_sequence;
INSERT INTO sqlite_sequence VALUES('proposal_log',52);
COMMIT;
