namespace LesioBlog2_Repo.Abstract
{
    public  interface ICodeRepo
    {
        int AddCode(int userId);
        void SaveChanges();
        int? GetCodeValue(int id);
    }
}
