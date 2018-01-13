
CREATE VIEW dbo.ResourceSurvey AS
SELECT
  rs.ResourceUID
, rs.SkillUID
, rs.AspectUID
, rs.Ratingvalue
, rs.SnapshotTimestamp
FROM
(
  SELECT
    rsx.*
  , ROW_NUMBER() OVER (
      PARTITION BY rsx.ResourceUID, rsx.SkillUID, rsx.AspectUID
      ORDER BY rsx.SnapshotTimestamp Desc) AS FilterIndex
  FROM
  dbo.ResourceSurveySnapshot rsx

) rs
WHERE
rs.FilterIndex = 1
;