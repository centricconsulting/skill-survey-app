CREATE VIEW dbo.SurveyResponse AS
SELECT
  rs.ResourceUID
, rs.SkillUID
, rs.AspectUID
, rs.RatingValue
, rs.SnapshotTimestamp
FROM
(
  SELECT
    rsx.*
  , ROW_NUMBER() OVER (
      PARTITION BY rsx.ResourceUID, rsx.SkillUID, rsx.AspectUID
      ORDER BY rsx.SnapshotTimestamp Desc) AS FilterIndex
  FROM
  dbo.SurveyResponseSnapshot rsx

) rs
WHERE
rs.FilterIndex = 1
;