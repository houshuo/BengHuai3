namespace MoleMole
{
    using proto;
    using System;

    public class MailCacheKey
    {
        public int id;
        public DateTime time;
        public MailType type;

        public MailCacheKey()
        {
            this.type = 3;
            this.time = DateTime.Now;
        }

        public MailCacheKey(MailType type, int id, DateTime time)
        {
            this.type = 3;
            this.time = DateTime.Now;
            this.type = type;
            this.id = id;
            this.time = time;
        }
    }
}

