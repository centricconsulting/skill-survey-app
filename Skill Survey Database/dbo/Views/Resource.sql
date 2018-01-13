
CREATE VIEW dbo.[Resource] AS
SELECT
  r.ResourceUID
, r.EmployerDesc
, r.CentricEmployerFlag
, r.FirstName
, r.LastName 
, r.ResourceLabel
, r.EmailAddress
, r.CreateTimestamp
, r.ModifyTimestamp
FROM
(
  SELECT rs.*
  , ROW_NUMBER() OVER (
      PARTITION BY rs.ResourceUID
      ORDER BY rs.SnapshotTimestamp DESC) AS FilterIndex
  FROM
  dbo.[ResourceSnapshot] rs
) r
WHERE
r.FilterIndex = 1
;