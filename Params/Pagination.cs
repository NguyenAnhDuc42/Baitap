using Microsoft.EntityFrameworkCore;

namespace test.Params
{
    public class Pagination<T> : List<T>
    {
        public int TrangHienTai { get; set; }
        public int TongTrang { get; set; }
        public int TongItems { get; set; }
        public int KichCoTrang { get; set; }

        public Pagination(List<T> items,int tongItems,int trangHienTai, int kichCoTrang)
        {
            TrangHienTai = trangHienTai;
            TongTrang = (int)Math.Ceiling(tongItems / (double)kichCoTrang);
            TongItems = tongItems;
            KichCoTrang = kichCoTrang;

            this.AddRange(items);

        }

        public bool CoTrangTruoc => TrangHienTai > 1;
        public bool CoTrangSau => TrangHienTai < TongTrang;

        public static async Task<Pagination<T>> CreateAsync(IQueryable<T> data,int trangHienTai,int kichCoTrang)
        {
            var tongItems = await data.CountAsync();
            var items = await data.Skip((trangHienTai - 1 ) * kichCoTrang).Take(kichCoTrang).ToListAsync();

            return new Pagination<T>(items, tongItems, trangHienTai, kichCoTrang);
        }
    }

}
