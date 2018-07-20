using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GuvenXYZApp
{
    class Program
    {
        static List<Koordinat> _koordinat;
        static List<Koordinat> _testList;
        static void Main(string[] args)
        {
            Console.Write("Dosya Yolu:");
            string dosyayolu = Console.ReadLine();
            _testList = new List<Koordinat>();
            CSRS_PPP(dosyayolu);
            OPUS(dosyayolu);

            int say = 0;
            _testList.ForEach(v =>
            {
                say++;
                Console.WriteLine($"[{say}] \t{v.DosyaAdi} \t {v.X?.Replace("\t", "").Replace("(", "")} \t {v.Y?.Replace("\t", "").Replace("(", "")} \t {v.Z?.Replace("\t", "").Replace("(", "")}");
            });
            say = 0;
            _testList.ForEach(v =>
            {
                say++;
                Console.WriteLine($"[{say}] \t{v.DosyaAdi} \t {v.XM?.Replace("\t", "").Replace("�", "±")} \t {v.YM?.Replace("\t", "").Replace("�", "±")} \t {v.ZM?.Replace("\t", "").Replace("�", "±")}");
            });

            Console.ReadLine();
        }

        static void CSRS_PPP(string yol)
        {
            Dosyalar($@"{yol}\CSRS_PPP", EKlasor.CSRS_PPP);
            _testList.Add(new Koordinat() { DosyaAdi = "-------------", X = "--------", Y = "--------" });
        }

        static void OPUS(string yol)
        {
            Dosyalar($@"{yol}\OPUS", EKlasor.OPUS);
            Console.WriteLine($"OPUS");
        }

        public static void Dosyalar(string yol, EKlasor yer, EOzelDurum ozelDurum = 0)
        {
            List<string> dosyalar = Directory.GetFiles(yol).ToList();

            foreach (string dosya in dosyalar)
            {
                Koordinat krd = new Koordinat();

                FileInfo dosyaBilgi = new FileInfo(dosya);
                krd.DosyaAdi = dosyaBilgi.Name;
                if (krd.DosyaAdi?.IndexOf("Koordinat.txt") > 0 || krd.DosyaAdi?.IndexOf("Koordinat.txt") > 0)
                {
                    break;
                }
                else
                {
                    using (FileStream dosyaAc = dosyaBilgi.OpenRead())
                    {
                        using (StreamReader dosyaOku = new StreamReader(dosya))
                        {
                            string satir;
                            int satirSay = 1;

                            while ((satir = dosyaOku.ReadLine()) != null)
                            {

                                switch (satirSay)
                                {
                                    case 14:
                                        if (EKlasor.CSRS_PPP == yer && EOzelDurum.TABL != ozelDurum)
                                        {
                                            krd.X = satir.Substring(1, 12);
                                            krd.XM = satir.Substring(13, 6);
                                        }
                                        break;
                                    case 15:
                                        if (EKlasor.CSRS_PPP == yer && EOzelDurum.TABL != ozelDurum)
                                        {
                                            krd.Y = satir.Substring(2, 12);
                                            krd.YM = satir.Substring(14, 6);
                                        }
                                        else
                                        {
                                            krd.X = satir.Substring(2, 12);
                                            krd.XM = satir.Substring(14, 6);
                                        }
                                        break;
                                    case 16:
                                        if (EKlasor.CSRS_PPP == yer && EOzelDurum.TABL != ozelDurum)
                                        {
                                            krd.Z = satir.Substring(2, 12);
                                            krd.ZM = satir.Substring(14, 6);
                                        }
                                        else
                                        {
                                            krd.Y = satir.Substring(2, 12);
                                            krd.YM = satir.Substring(14, 6);
                                        }
                                        break;
                                    case 17:
                                        if (EKlasor.CSRS_PPP == yer && EOzelDurum.TABL == ozelDurum)
                                        {
                                            krd.Z = satir.Substring(2, 12);
                                            krd.ZM = satir.Substring(14, 6);
                                        }
                                        break;
                                    case 22:
                                        if (EKlasor.OPUS == yer)
                                        {
                                            krd.X = satir.Substring(17, 12);
                                            krd.XM = satir.Substring(34, 5);
                                        }
                                        break;
                                    case 23:
                                        if (EKlasor.OPUS == yer)
                                        {
                                            krd.Y = satir.Substring(17, 12);
                                            krd.YM = satir.Substring(34, 5);
                                        }
                                        break;
                                    case 24:
                                        if (EKlasor.OPUS == yer)
                                        {
                                            krd.Z = satir.Substring(17, 12);
                                            krd.ZM = satir.Substring(34, 5);
                                        }
                                        break;
                                }
                                satirSay++;
                            }
                            _koordinat.Add(krd);
                            _testList.Add(krd);
                        }
                    }
                }
            }

            List<string> klasorler = Directory.GetDirectories(yol).ToList();

            foreach (var klasor in klasorler)
            {

                _koordinat = new List<Koordinat>();
                string kontrol = klasor.Substring((klasor.Length - 4), 4);
                switch (kontrol)
                {
                    case "TABL":
                        Dosyalar(klasor, yer, EOzelDurum.TABL);
                        break;
                    default:
                        Dosyalar(klasor, yer);
                        break;
                }

                if (_koordinat.Any())
                {
                    string yenidosyaKordinat = $@"{klasor}\\{kontrol}-Koordinat.txt";
                    using (FileStream dosyaOlustur = new FileStream(yenidosyaKordinat, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(dosyaOlustur))
                        {
                            _koordinat.ForEach(v =>
                            {
                                sw.WriteLine($"{v.DosyaAdi?.Replace(".txt", "")} \t {v.X?.Replace("\t", "").Replace("(", "")} \t {v.Y?.Replace("\t", "").Replace("(", "")} \t {v.Z?.Replace("\t", "").Replace("(", "")}");
                            });
                            sw.Flush();
                        }
                    }
                    string yenidosyaSapma = $@"{klasor}\\{kontrol}-Sapma.txt";
                    using (FileStream dosyaOlustur = new FileStream(yenidosyaSapma, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(dosyaOlustur))
                        {
                            _koordinat.ForEach(v =>
                            {
                                sw.WriteLine($"{v.DosyaAdi?.Replace(".txt", "")} \t {v.XM?.Replace("\t", "").Replace("�", "±")} \t {v.YM?.Replace("\t", "").Replace("�", "±")} \t {v.ZM?.Replace("\t", "").Replace("�", "±")}");
                            });
                            sw.Flush();
                        }
                    }
                }
            }
        }
    }
}
public enum EOzelDurum
{
    Degersiz,
    TABL,
}
public enum EKlasor
{
    CSRS_PPP,
    OPUS
}
public class Koordinat
{
    public string DosyaAdi { get; set; }
    public string X { get; set; }
    public string XM { get; set; }
    public string Y { get; set; }
    public string YM { get; set; }
    public string Z { get; set; }
    public string ZM { get; set; }
}
