/****** Script for SelectTopNRows command from SSMS  ******/




  
  select * from Wpis;  
  
  
  delete Wpis from Wpis where Wpis.WpisID = 22;
  
  --- if plusowal wpis:
  select * from IfPlusowalWpis where IfPlusowalWpis.WpisID = 26;
  
  delete IfPlusowalWpis from IfPlusowalWpis where IfPlusowalWpis.WpisID = 31;
  --- tagowanieTagWpis
  select * from WpisTag;
  delete WpisTag from WpisTag where WpisTag.WpisID = 22;
   --- jak robie delete wpisu z tagami komentami itp to delete ifpluswpis tez zrobic query
 -- ifplus comment sie wywala najpierw
 -- potem ifpluswpis 
   
   select * from IfPlusowalComment;
  
  select * from IfPlusowalWpis;
  
  
  select * from Comment;
  
  delete Comment from Comment where Comment.CommentID = 28;
  
  select * from Comment where Comment.CommentID = 28;
  
  select * from IfPlusowalComment;
  
  delete IfPlusowalComment from IfPlusowalComment where IfPlusowalComment.CommentID = 28;
  