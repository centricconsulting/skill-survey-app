using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Design;
using System.Configuration;


namespace Centric.SkillSurvey.Repositories
{

  public interface IRepository<TEntity> where TEntity : class
  {
    IEnumerable<TEntity> SelectAll(Func<TEntity, bool> WhereClause, PageRange PageRange);
    IEnumerable<TEntity> SelectAll(Func<TEntity, bool> WhereClause);
    IEnumerable<TEntity> SelectAll(PageRange PageRange);
    IEnumerable<TEntity> SelectAll();
    TEntity SelectSingle(object id);
    TEntity SelectSingle(Func<TEntity, bool> WhereClause);
    void Insert(TEntity obj);
    void InsertAll(IEnumerable<TEntity> obj);
    void Update(TEntity obj);
    void UpdateOnly(TEntity obj, string[] includeProperties);
    void UpdateExcept(TEntity obj, string[] excludeProperties);
    void DeleteSingle(object id);
    void DeleteAll(Func<TEntity, bool> WhereClause);
    void Save();
  }

  public class PageRange
  {
    public PageRange() { }
    public PageRange(int Page, int ItemsPerPage)
    {
      this.Page = Page;
      this.ItemsPerPage = ItemsPerPage;
    }

    public int Page { get; set; }
    public int ItemsPerPage { get; set; }

    public int Skip
    {
      get { return ItemsPerPage * (Page - 1); }
    }
  }

  public abstract class BaseRepository<TEntity> : IDisposable, IRepository<TEntity> where TEntity : class
  {

    public ApplicationContext AppContext;
    public virtual DbSet<TEntity> Set { get; set; }

    public BaseRepository(ApplicationContext AppContext)
    {
      this.AppContext = AppContext;
      this.Set = this.AppContext.Set<TEntity>();
    }

    public IEnumerable<TEntity> SelectAll(PageRange PageRange)
    {
      return this.Set.Skip(PageRange.Skip).Take(PageRange.ItemsPerPage);
    }

    public IEnumerable<TEntity> SelectAll(Func<TEntity, bool> WhereClause)
    {
      return this.Set.Where(WhereClause).ToList();
    }

    public IEnumerable<TEntity> SelectAll(Func<TEntity, bool> WhereClause, PageRange PageRange)
    {

      /* USAGE            
      Func<Address, bool> whereClause = address => address.Zip == 23456;
      var query = someList.Where(whereClause); 
       */

      return this.Set.Where(WhereClause).Skip(PageRange.Skip).Take(PageRange.ItemsPerPage).ToList();
    }


    public IEnumerable<TEntity> SelectAll()
    {
      return this.Set.ToList();
    }

    public TEntity SelectSingle(object id)
    {
      return this.Set.Find(id);
    }
    public TEntity SelectSingle(Func<TEntity, bool> WhereClause)
    {
      return this.Set.Where(WhereClause).FirstOrDefault();
    }

    public void Insert(TEntity obj)
    {
      this.Set.Add(obj);
      this.Save();
    }

    public void InsertAll(IEnumerable<TEntity> obj)
    {

      this.Set.AddRange(obj);
      this.Save();
    }

    public void Update(TEntity obj)
    {
      this.Set.Attach(obj);
      AppContext.Entry(obj).State = EntityState.Modified;
      this.Save();
    }

    public void UpdateOnly(TEntity obj, string[] includeProperties)
    {
      if (includeProperties == null)
      {
        Update(obj);
      }
      else
      {

        this.Set.Attach(obj);

        // set entity to unchanged, unset each included property
        AppContext.Entry(obj).State = EntityState.Unchanged;
        AppContext.Entry(obj).Property(x => includeProperties.Contains(x.ToString())).IsModified = true;

        this.Save();
      }
    }


    public void UpdateExcept(TEntity obj, string[] excludeProperties)
    {

      if (excludeProperties == null)
      {
        Update(obj);
      }
      else
      {
        this.Set.Attach(obj);

        // set entity to modified, unset each excluded property
        AppContext.Entry(obj).State = EntityState.Modified;
        AppContext.Entry(obj).Property(x => excludeProperties.Contains(x.ToString())).IsModified = false;

        this.Save();
      }

    }

    public void DeleteSingle(object id)
    {
      TEntity existing = this.Set.Find(id);
      this.Set.Remove(existing);
      this.Save();
    }

    public void DeleteAll(Func<TEntity, bool> WhereClause)
    {
      this.Set.Where(WhereClause).ToList().ForEach(x => this.Set.Remove(x));
      this.Save();
    }

    public void Save()
    {
      AppContext.SaveChanges();
    }


    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        if (disposing)
        {
          AppContext.Dispose();
        }
      }
      this.disposed = true;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

  }
}

