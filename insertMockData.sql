BEGIN TRANSACTION;

-- Lists
INSERT INTO Lists ("id","title") VALUES (1,'Work');
INSERT INTO Lists ("id","title") VALUES (2,'Home');
INSERT INTO Lists ("id","title") VALUES (3,'Bills');
INSERT INTO Lists ("id","title") VALUES (4,'Groceries');

-- Tasks
INSERT INTO Tasks VALUES (1,'Prepare office report','Finish the office report before Friday.', date('now','+3 days'), NULL,0,1,1);
INSERT INTO Tasks VALUES (2,'Go to the office tomorrow','Important meeting. Demain. Bureau.', date('now','+1 day'), NULL,0,1,1);
INSERT INTO Tasks VALUES (3,'Electricity bill','Pay before Friday.', date('now','+4 days'), NULL,0,3,1);
INSERT INTO Tasks VALUES (4,'Buy vegetables','Carrots, tomatoes, onions. Légumes.', date('now','-1 day'), NULL,0,4,1);
INSERT INTO Tasks VALUES (5,'Kitchen cleaning today','Deep cleaning.', date('now','-1 day'), NULL,0,2,1);
INSERT INTO Tasks VALUES (6,'Clean kitchen','Daily routine.', date('now'), NULL,0,2,1);
INSERT INTO Tasks VALUES (7,'Factura lunes','Pago mensual.', date('now','+3 days'), NULL,0,3,1);
INSERT INTO Tasks VALUES (8,'Plan next week tasks','Weekly planning.', date('now','+7 days'), NULL,0,1,1);
INSERT INTO Tasks VALUES (9,'March rent','Monthly payment. Hier.', date('now','-5 days'), NULL,0,3,1);
INSERT INTO Tasks VALUES (10,'Prepare office report','(duplicate title allowed)', date('now','+3 days'), NULL,0,1,1);
INSERT INTO Tasks VALUES (11,'Pay rent March','Rent payment for March.', date('now','+27 days'), NULL,0,3,1);
INSERT INTO Tasks VALUES (12,'Payer l’impôt en avril','Impôt annuel prévu en avril.', date('now','+27 days'), NULL,0,3,1);
INSERT INTO Tasks VALUES (13,'Reunión en septiembre','Reunión importante en septiembre.', date('now','+6 months'), NULL,0,1,1);
INSERT INTO Tasks VALUES (14,'Fix broken window','Window broken since last week.', date('now','+7 days'), NULL,0,2,1);
INSERT INTO Tasks VALUES (15,'Old tax declaration','Déclaration fiscale de l’année passée. Declaración pasada.', date('now','-4 months'), NULL,0,3,1);
INSERT INTO Tasks VALUES (16,'Revisión anual 2027','Annual car inspection scheduled next year.', date('now','+1 year'), NULL,0,3,1);
INSERT INTO Tasks VALUES (17,'Revisión del coche','Revisión del coche mañana.', date('now','+1 day'), NULL,0,2,1);
INSERT INTO Tasks VALUES (18,'Comprar azúcar','Comprar azúcar / azucar / azucár.', date('now','+1 day'), NULL,0,4,1);
INSERT INTO Tasks VALUES (19,'Pay electricity February','Electricity bill for February.', date('now','-25 days'), NULL,0,3,1);
INSERT INTO Tasks VALUES (20,'Pay trash tax','Taxe poubbelle du bureau à payer ce mois.', date('now','+4 days'), NULL,0,3,1);
INSERT INTO Tasks VALUES (21,'Urgent tax review','Important fiscal review.', date('now','+2 days'), NULL,1,3,1);
INSERT INTO Tasks VALUES (22,'Weekly cleaning','Weekly repetitive cleaning routine.', date('now','+7 days'), NULL,3,2,1);
INSERT INTO Tasks VALUES (23,'Monthly budget','Monthly repetitive budget review.', date('now','+1 month'), NULL,3,3,1);
INSERT INTO Tasks VALUES (24,'Daily backup','Daily repetitive system backup.', date('now'), NULL,3,1,1);
INSERT INTO Tasks VALUES (25,'Weekly groceries','Weekly repetitive grocery list.', date('now','+7 days'), NULL,3,4,1);
INSERT INTO Tasks VALUES (26,'Charlotte','1984', date('now','+1 day'), NULL,4,2,1);
INSERT INTO Tasks VALUES (27,'John','1995', date('now','+7 days'), NULL,4,2,1);
INSERT INTO Tasks VALUES (28,'Sara','1988', date('now','+14 days'), NULL,4,2,1);
INSERT INTO Tasks VALUES (29,'Lucas','1982', date('now','+1 month'), NULL,4,2,1);
INSERT INTO Tasks VALUES (30,'Team sync Thursday','Weekly sync meeting.', date('now','weekday 4'), NULL,0,1,1);
INSERT INTO Tasks VALUES (31,'Call insurance','Appointment scheduled.', date('now','+2 days'), NULL,0,1,1);
INSERT INTO Tasks VALUES (32,'Dentist appointment','Routine dental check.', date('now','+14 days'), NULL,0,2,1);
INSERT INTO Tasks VALUES (33,'Grocery restock','Buy milk, eggs, bread.', date('now','-3 days'), NULL,0,4,1);
INSERT INTO Tasks VALUES (34,'Renew ID card','Administrative renewal.', date('now','+2 months'), NULL,0,1,1);
INSERT INTO Tasks VALUES (35,'Car insurance renewal','Annual renewal.', date('now','+3 months'), NULL,0,3,1);
INSERT INTO Tasks VALUES (36,'Submit tax documents','Fiscal submission.', date('now','+1 month'), NULL,1,3,1);
INSERT INTO Tasks VALUES (37,'Anna','1990', date('now','+3 months'), NULL,4,2,1);
INSERT INTO Tasks VALUES (38,'Pay water bill','Monthly water bill.', date('now','+6 days'), NULL,0,3,1);
INSERT INTO Tasks VALUES (39,'Prepare presentation','Slides for next meeting.', date('now','+6 days'), NULL,0,1,1);
INSERT INTO Tasks VALUES (40,'Doctor appointment','Routine check-up.', date('now','+12 days'), NULL,0,2,1);

COMMIT;
