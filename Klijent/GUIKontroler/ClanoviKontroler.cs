﻿using Klijent.UserKontrole;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zajednicko;
using Zajednicko.Domain;
using Zajednicko.Komunikacija;

namespace Klijent.GUIKontroler
{
    public class ClanoviKontroler
    {
        private UCClanovi kontrolaClanovi;
        List<Clan> clanovi;
        public Clan Clan { get; set; }
        public event EventHandler ClanObrisan;
        public event EventHandler FormirajLabelu;
        public event EventHandler FormirajLabeluSviRacuni;
        private static ClanoviKontroler instance;
        public static ClanoviKontroler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ClanoviKontroler();
                }
                return instance;
            }
        }
        private ClanoviKontroler()
        {
            kontrolaClanovi = new UCClanovi();
            kontrolaClanovi.BtnPretraga.Click += PretraziClana;
            kontrolaClanovi.BtnDetalji.Click += DetaljiOClanu;
            kontrolaClanovi.BtnObrisi.Click += ObrisiClana;
            kontrolaClanovi.BtnKreirajDolazak.Click += PostaviClana;
            kontrolaClanovi.BtnKreirajRacun.Click += PostaviClana;
            kontrolaClanovi.BtnSviRacuni.Click += PostaviClana;
            DodajClanaKontroler.Instance.ClanDodat += Instance_ClanDodat;
            ClanObrisan += ClanoviKontroler_ClanObrisan;
        }



        private void PostaviClana(object sender, EventArgs e)
        {
            if (kontrolaClanovi.DgvClanovi.SelectedRows.Count > 0)
            {

                DataGridViewRow red = kontrolaClanovi.DgvClanovi.SelectedRows[0];
                Clan izabraniClan = red.DataBoundItem as Clan;
                if (izabraniClan != null)
                {
                    Clan = izabraniClan;
                    //  EventHandler eh = (se, ee) => { DodajRacunKontroler.Instance.Forma_Load(se,ee); };
                    if (sender is Button && ((Button)sender).Name == "btnKreirajRacun")
                    {
                        FormirajLabelu?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        FormirajLabeluSviRacuni?.Invoke(this, EventArgs.Empty);
                    }
                }

            }
            else
            {
                MessageBox.Show("Morate izabrati red u tabeli.");
            }
        }

        private void ClanoviKontroler_ClanObrisan(object sender, EventArgs e)
        {
            PostaviDGV();
        }

        private void Instance_ClanDodat(object sender, EventArgs e)
        {
            PostaviDGV();
        }

        private void ObrisiClana(object sender, EventArgs e)
        {
            if (kontrolaClanovi.DgvClanovi.SelectedRows.Count > 0)
            {
              //  DialogResult rezultat = MessageBox.Show("Da li ste sigurni?", "Upozorenje", MessageBoxButtons.YesNo);
               // if (rezultat == DialogResult.Yes)
               // {
                    DataGridViewRow red = kontrolaClanovi.DgvClanovi.SelectedRows[0];
                    Clan izabraniClan = red.DataBoundItem as Clan;
                    if (izabraniClan != null)
                    {
                        
                       DodajClanaKontroler.Instance.PrikaziDetaljeOClanuBrisanje(izabraniClan);
                      
                        Komunikacija.Instance.IzvrsiFju(Operacija.IzbrisiClana, izabraniClan);
                       // MessageBox.Show($"Član {izabraniClan.Ime} {izabraniClan.Prezime} je izbrisan");
                        ClanObrisan?.Invoke(this, EventArgs.Empty);
                    }
                //}
            }
            else
            {
                MessageBox.Show("Morate izabrati red u tabeli.");
            }
        }

        private void DetaljiOClanu(object sender, EventArgs e)
        {
            if (kontrolaClanovi.DgvClanovi.SelectedRows.Count > 0)
            {
                DataGridViewRow red = kontrolaClanovi.DgvClanovi.SelectedRows[0];
                Clan izabraniClan = red.DataBoundItem as Clan;

                if (izabraniClan != null)
                {
                    Racun ra = new Racun();
                    ra.Clan = izabraniClan;
                    ra.Mesec =(Mesec) DateTime.Now.Month;
                    ra.Godina =DateTime.Now.Year;
                  Odgovor odg=  Komunikacija.Instance.IzvrsiFju(Operacija.PronadjiRacune,ra);
                    if (((List<Racun>)odg.Rezultat).Count == 0)
                    {
                        DodajClanaKontroler.Instance.PrikaziDetaljeOClanu(izabraniClan);
                    }
                    else
                    {
                        MessageBox.Show("Ne možete promeniti promeniti podatke o članu kom je već kreirana " +
                            "članarina za ovaj mesec!");
                    }
                }

            }
            else
            {
                MessageBox.Show("Morate izabrati člana!");
            }
        }


        public UCClanovi vratiKontroluClanovi()
        {
            PostaviDGV();
            return kontrolaClanovi;
        }
        public void PostaviDGV()
        {
            clanovi = (List<Clan>)
               Komunikacija.Instance.IzvrsiFju(Operacija.VratiSveClanove).Rezultat;
            clanovi = clanovi.OrderBy(c => c.Prezime).ToList();
            kontrolaClanovi.DgvClanovi.DataSource = new BindingList<Clan>(clanovi);
            kontrolaClanovi.DgvClanovi.Columns["idclana"].Visible = false;

            kontrolaClanovi.DgvClanovi.Columns["imetabele"].Visible = false;
            kontrolaClanovi.DgvClanovi.Columns["vrednosti"].Visible = false;
            kontrolaClanovi.DgvClanovi.Columns["UslovZaWhereVratiSve"].Visible = false;
            kontrolaClanovi.DgvClanovi.Columns["izrazzajoin"].Visible = false;
            kontrolaClanovi.DgvClanovi.Columns["UslovZaDelete"].Visible = false;
            kontrolaClanovi.DgvClanovi.Columns["UslovZaUpdate"].Visible = false;
            kontrolaClanovi.DgvClanovi.Columns["UslovZaSet"].Visible = false;
            kontrolaClanovi.DgvClanovi.Columns["UslovZaWhereSaUslovom"].Visible = false;
            kontrolaClanovi.DgvClanovi.Columns["Identifikator"].Visible = false;
        }

        internal void PretraziClana(object sender, EventArgs e)
        {
            string unos = kontrolaClanovi.TxtUnos.Text;
            if (!string.IsNullOrEmpty(unos))
            {
                if (unos.All(char.IsLetter))
                {
                    Clan c = new Clan();
                    c.Prezime = unos;
                    List<Clan> dgvCl = new List<Clan>();
                    List<Clan> odabraniClanovi = (List<Clan>)
                      Komunikacija.Instance.IzvrsiFju(Operacija.VratiKonkretneClanove, c).Rezultat;
                    if (odabraniClanovi.Count == 0)
                    {
                        kontrolaClanovi.DgvClanovi.DataSource = odabraniClanovi;
                        MessageBox.Show("Sistem ne može da nađe članove po zadatoj vrednosti");
                       

                    }
                    else
                    {
                        foreach (Clan cl in clanovi)
                        {
                            foreach (Clan cla in odabraniClanovi)
                            {
                                if (cla.IdClana == cl.IdClana)
                                {
                                    dgvCl.Add(cla);
                                    kontrolaClanovi.DgvClanovi.DataSource = new BindingList<Clan>(dgvCl);
                                }
                            }
                        }
                        MessageBox.Show("Sistem je našao članove po zadatoj vrednosti.");
                    }
                }
                else
                {
                    MessageBox.Show("Unesite samo slova!");
                }
            }
            else
            {
                kontrolaClanovi.DgvClanovi.DataSource = (List<Clan>)
                      Komunikacija.Instance.IzvrsiFju(Operacija.VratiSveClanove).Rezultat;
            }
        }
    }
}
