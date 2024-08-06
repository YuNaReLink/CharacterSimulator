
public class CountUp
{
    private int count = 0;
    private bool addflag = false;
    public bool IsAddFlag {  get { return addflag; } set { addflag = value; } }
    public bool IsMaxCount(int maxcount) { return count >= maxcount; }
    public void InitCount() { count = 0; }
    public void AddCount(int _count,int _maxcount)
    {
        addflag = true;
        count += _count;
        if(count > _maxcount)
        {
            count = _maxcount;
        }
    }

    public void Update()
    {
        count--;
    }
    
}
