USE [RPPP02]
GO
INSERT [dbo].[VrstaSustava] ([Naziv_vrste_sustava]) VALUES (N'Mrezni sustav')
INSERT [dbo].[VrstaSustava] ([Naziv_vrste_sustava]) VALUES (N'Sustav radara')
INSERT [dbo].[VrstaSustava] ([Naziv_vrste_sustava]) VALUES (N'Sustav za mjerenje temperature')

GO
INSERT [dbo].[Kriticnost] ([Stupanj_kriticnosti]) VALUES (N'Kritican')
INSERT [dbo].[Kriticnost] ([Stupanj_kriticnosti]) VALUES (N'Nije kritican')
INSERT [dbo].[Kriticnost] ([Stupanj_kriticnosti]) VALUES (N'Slabo kritican')
INSERT [dbo].[Kriticnost] ([Stupanj_kriticnosti]) VALUES (N'Srednje kritican')
GO
INSERT [dbo].[Sustav] ([Opis], [ID_VrstaSustava], [ID_Kriticnost] , [Osjetljivost]) VALUES (N'Sustav koji se spaja na internet', 1, 1, 4)
INSERT [dbo].[Sustav] ([Opis], [ID_VrstaSustava], [ID_Kriticnost] , [Osjetljivost]) VALUES (N'Sustav koji sinkronizira radare', 2, 2, 8)
GO

INSERT [dbo].[Lokacija] ([Naziv], [Postanski_broj], [Kontakt_telefon]) VALUES (N'Zagreb', N'10000', N'+385996987845')
INSERT [dbo].[Lokacija] ([Naziv], [Postanski_broj], [Kontakt_telefon]) VALUES (N'Zadar', N'23000', N'+385951697462')
INSERT [dbo].[Lokacija] ([Naziv], [Postanski_broj], [Kontakt_telefon]) VALUES (N'Vinkovci', N'32100', N'+385977493066')
INSERT [dbo].[Lokacija] ([Naziv], [Postanski_broj], [Kontakt_telefon]) VALUES (N'Slavonski Brod', N'35000', N'+38659776532')
GO
INSERT [dbo].[Podsustav] ([Ucestalost_odrzavanja], [Osjetljivost], [Naziv], [Opis], [ID_Sustav], [ID_Lokacija], [ID_Kriticnost]) VALUES (3, 4, N'Server room', N'Soba u kojoj se nalazi server i svi potrebni uredaji', 1, 1, 1)
INSERT [dbo].[Podsustav] ([Ucestalost_odrzavanja], [Osjetljivost], [Naziv], [Opis], [ID_Sustav], [ID_Lokacija], [ID_Kriticnost]) VALUES (4, 3, N'Clients room', N'Soba s klijentskom stranom sustava', 1, 2, 3)
INSERT [dbo].[Podsustav] ([Ucestalost_odrzavanja], [Osjetljivost], [Naziv], [Opis], [ID_Sustav], [ID_Lokacija], [ID_Kriticnost]) VALUES (5, 2, N'Radars - North', N'Sjeverna strana luke s radarima', 2, 2, 2)
INSERT [dbo].[Podsustav] ([Ucestalost_odrzavanja], [Osjetljivost], [Naziv], [Opis], [ID_Sustav], [ID_Lokacija], [ID_Kriticnost]) VALUES (5, 2, N'Radars - South', N'Južna strana luke s radarima', 2, 1, 4)

GO

INSERT [dbo].[Funkcije] ([Kategorija], [Naziv], [ID_Podsustav]) VALUES (N'Online funkcija',  N'Data recording', 1)
INSERT [dbo].[Funkcije] ([Kategorija], [Naziv], [ID_Podsustav]) VALUES (N'Operativna funkcija', N'Multi radar tracking', 4)

GO
INSERT [dbo].[PodrucjeRada] ([Vrsta_podrucja_rada]) VALUES (N'Hardver')
INSERT [dbo].[PodrucjeRada] ([Vrsta_podrucja_rada]) VALUES (N'Racunalna periferija')
INSERT [dbo].[PodrucjeRada] ([Vrsta_podrucja_rada]) VALUES (N'Softver')
INSERT [dbo].[PodrucjeRada] ([Vrsta_podrucja_rada]) VALUES (N'Termoinstalacije')
GO

INSERT [dbo].[TimZaOdrzavanje] ([Datum_osnivanja], [Naziv_tima], [Satnica], [ID_PodrucjeRada]) VALUES (CAST(N'2015-08-14' AS Date), N'TheBestTeam', 150, 1)
INSERT [dbo].[TimZaOdrzavanje] ([Datum_osnivanja], [Naziv_tima], [Satnica], [ID_PodrucjeRada]) VALUES (CAST(N'1862-06-18' AS Date), N'Tim1862', 24.99, 2)
INSERT [dbo].[TimZaOdrzavanje] ([Datum_osnivanja], [Naziv_tima], [Satnica], [ID_PodrucjeRada]) VALUES (CAST(N'2020-03-13' AS Date), N'TimBezPosla', 1, 2)
GO

INSERT [dbo].[PlanOdrzavanja] ([Razina_strucnosti], [Datum_odrzavanja], [ID_Podsustav], [ID_TimZaOdrzavanje]) VALUES (2, CAST(N'2021-11-06' AS Date), 1, 1)
INSERT [dbo].[PlanOdrzavanja] ([Razina_strucnosti], [Datum_odrzavanja], [ID_Podsustav], [ID_TimZaOdrzavanje]) VALUES (1, CAST(N'2001-10-10' AS Date), 2, 2)

GO
INSERT [dbo].[PlaniranZa] ([ID_Lokacija], [ID_PlanOdrzavanja]) VALUES (1, 2)
INSERT [dbo].[PlaniranZa] ([ID_Lokacija], [ID_PlanOdrzavanja]) VALUES (2, 1)
GO
INSERT [dbo].[StrucnaSprema] ([Razina_strucne_spreme]) VALUES (N'Srednja strucna sprema')
INSERT [dbo].[StrucnaSprema] ([Razina_strucne_spreme]) VALUES (N'Visa strucna sprema')
INSERT [dbo].[StrucnaSprema] ([Razina_strucne_spreme]) VALUES (N'Visoka strucna sprema')
GO

INSERT [dbo].[Radnik] ([Certifikat], [Istek_certifikata], [Dezuran], [ID_TimZaOdrzavanje], [ID_StrucnaSprema], [Ime], [Prezime]) VALUES (NULL, NULL, 0, 1, 1, N'Marko', N'Tunjic')
INSERT [dbo].[Radnik] ([Certifikat], [Istek_certifikata], [Dezuran], [ID_TimZaOdrzavanje], [ID_StrucnaSprema], [Ime], [Prezime]) VALUES (N'NABCEP (North American Board of Certified Energy Practitioners)', CAST(N'2030-01-01' AS Date), 0, 1, 1, N'Zdravko', N'Petripusic')
INSERT [dbo].[Radnik] ([Certifikat], [Istek_certifikata], [Dezuran], [ID_TimZaOdrzavanje], [ID_StrucnaSprema], [Ime], [Prezime]) VALUES (N'STC (Solar Thermal Certificate)', NULL, 0, 1, 1, N'Fran', N'Periskop')
INSERT [dbo].[Radnik] ([Certifikat], [Istek_certifikata], [Dezuran], [ID_TimZaOdrzavanje], [ID_StrucnaSprema], [Ime], [Prezime]) VALUES (N'CroControl', CAST(N'2025-07-01' AS Date), 1, 3, 2, N'Nikola', N'Tesla')
INSERT [dbo].[Radnik] ([Certifikat], [Istek_certifikata], [Dezuran], [ID_TimZaOdrzavanje], [ID_StrucnaSprema], [Ime], [Prezime]) VALUES (NULL, NULL, 1, 2, 2, N'Blaženko', N'Sitz')

GO

INSERT [dbo].[TipOpreme] ([Tip_opreme]) VALUES (N'Mrezna oprema')
INSERT [dbo].[TipOpreme] ([Tip_opreme]) VALUES (N'Oprema za vjetar')
INSERT [dbo].[TipOpreme] ([Tip_opreme]) VALUES (N'Termalna oprema')
GO
INSERT [dbo].[Stanje] ([Tip_stanja]) VALUES (N'Ispravan')
INSERT [dbo].[Stanje] ([Tip_stanja]) VALUES (N'Neispravan')
INSERT [dbo].[Stanje] ([Tip_stanja]) VALUES (N'Servisira se')
INSERT [dbo].[Stanje] ([Tip_stanja]) VALUES (N'Servisiran')
GO

INSERT [dbo].[Oprema] ([Redundantnost], [Budzet], [Datum_pustanja_u_pogon], [ID_Podsustav], [ID_TipOpreme]) VALUES (10, 1000000, CAST(N'1995-01-01' AS Date), 1, 1)
INSERT [dbo].[Oprema] ([Redundantnost], [Budzet], [Datum_pustanja_u_pogon], [ID_Podsustav], [ID_TipOpreme]) VALUES (5, 50000, CAST(N'1995-01-01' AS Date), 2, 3)
INSERT [dbo].[Oprema] ([Redundantnost], [Budzet], [Datum_pustanja_u_pogon], [ID_Podsustav], [ID_TipOpreme]) VALUES (6, 15000, CAST(N'1995-01-01' AS Date), 3, 2)

GO


INSERT [dbo].[Uredaj] ([Naziv], [Proizvodac], [Godina_proizvodnje], [ID_Oprema], [ID_Stanje]) VALUES (N'Klima uredaj', N'Gorenje', N'2006', 1, 1)
INSERT [dbo].[Uredaj] ([Naziv], [Proizvodac], [Godina_proizvodnje], [ID_Oprema], [ID_Stanje]) VALUES (N'Anemometar', N'Probus', N'2019', 2, 3)
INSERT [dbo].[Uredaj] ([Naziv], [Proizvodac], [Godina_proizvodnje], [ID_Oprema], [ID_Stanje]) VALUES (N'Switch', N'DLink', N'2021', 3, 2)

GO
INSERT [dbo].[Informacija] ([Poruka], [Datum_slanja], [ID_Uredaj]) VALUES (N'Pokvaren sam', CAST(N'2021-01-01' AS Date), 1)
INSERT [dbo].[Informacija] ([Poruka], [Datum_slanja], [ID_Uredaj]) VALUES (N'Servis gotov', CAST(N'2021-11-08' AS Date), 2)
INSERT [dbo].[Informacija] ([Poruka], [Datum_slanja], [ID_Uredaj]) VALUES (N'Trebam servis', CAST(N'2021-11-07' AS Date), 3)
GO


INSERT [dbo].[Prirucnik] ([Izdavac], [Verzija], [Datum], [Naziv], [ID_Uredaj]) VALUES (N'Gorenje', 1, CAST(N'2001-01-01' AS Date), N'Prirucnik za gorenje klima uredaj', 2)
INSERT [dbo].[Prirucnik] ([Izdavac], [Verzija], [Datum], [Naziv], [ID_Uredaj]) VALUES (N'DLink', 2, CAST(N'2020-01-01' AS Date), N'Switch manual', 3)
INSERT [dbo].[Prirucnik] ([Izdavac], [Verzija], [Datum], [Naziv], [ID_Uredaj]) VALUES (N'Probus', 3, CAST(N'2020-01-01' AS Date), N'Upravljanje anemometrom', 1)

GO
INSERT [dbo].[Kvar] ([Tip], [Ozbiljnost], [Opis], [ID_Prirucnik]) VALUES (N'Tehnicki', 50, N'Anemometar predstavlja buru kao maestral', 3)
INSERT [dbo].[Kvar] ([Tip], [Ozbiljnost], [Opis], [ID_Prirucnik]) VALUES (N'Tehnicki', 10, N'Klima uredaj nece da se upali', 1)
GO
INSERT [dbo].[IzbioKvar] ([ID_Uredaj], [ID_Kvar]) VALUES (1, 2)
INSERT [dbo].[IzbioKvar] ([ID_Uredaj], [ID_Kvar]) VALUES (3, 1)
GO
INSERT [dbo].[NoviKvar] ([Postupak_rjesavanja], [Tip], [Ozbiljnost], [Opis]) VALUES (N'Kalibriraj strane svijeta', N'Tehnicki', 50, N'Cudno ponašanje mjeritelja vjetrova')
INSERT [dbo].[NoviKvar] ([Postupak_rjesavanja], [Tip], [Ozbiljnost], [Opis]) VALUES (N'Iskopcaj iz zida', N'Tehnicki', 1, N'Klima uredaj nece da se ugasi')
GO
INSERT [dbo].[UnosiSeKvar] ([ID_Prirucnik], [ID_NoviKvar]) VALUES (1, 1)
INSERT [dbo].[UnosiSeKvar] ([ID_Prirucnik], [ID_NoviKvar]) VALUES (3, 1)
GO
INSERT [dbo].[Stavka] ([Koraci], [Namjena], [ID_Prirucnik]) VALUES (N'1. Obrisi prasinu 2. pregledaj kablove', N'Odrzavanje', 1)
INSERT [dbo].[Stavka] ([Koraci], [Namjena], [ID_Prirucnik]) VALUES (N'1. Kalibriraj 2. Ponovo ukljuci', N'Popravak', 3)
GO

INSERT [dbo].[NadleznoTijelo] ([Naziv], [Tip]) VALUES (N'DLink', N'Interno')
INSERT [dbo].[NadleznoTijelo] ([Naziv], [Tip]) VALUES (N'Gorenje', N'Interno')
INSERT [dbo].[NadleznoTijelo] ([Naziv], [Tip]) VALUES (N'Probus', N'Interno')
GO

INSERT [dbo].[KrajSmjene] ([Vrijeme_kraja_smjene]) VALUES (4)
INSERT [dbo].[KrajSmjene] ([Vrijeme_kraja_smjene]) VALUES (8)
INSERT [dbo].[KrajSmjene] ([Vrijeme_kraja_smjene]) VALUES (12)
INSERT [dbo].[KrajSmjene] ([Vrijeme_kraja_smjene]) VALUES (16)
INSERT [dbo].[KrajSmjene] ([Vrijeme_kraja_smjene]) VALUES (20)
INSERT [dbo].[KrajSmjene] ([Vrijeme_kraja_smjene]) VALUES (24)
GO
INSERT [dbo].[Smjena] ([Pocetak_smjene], [Platni_faktor], [ID_KrajSmjene]) VALUES (N'4', 2, 3)
GO
INSERT [dbo].[Rang] ([Ime_ranga]) VALUES (N'Entry level')
INSERT [dbo].[Rang] ([Ime_ranga]) VALUES (N'Junior')
INSERT [dbo].[Rang] ([Ime_ranga]) VALUES (N'Mid')
INSERT [dbo].[Rang] ([Ime_ranga]) VALUES (N'Senior')
GO


INSERT [dbo].[Kontrolor] ([Ime], [Prezime], [OIB], [Datum_zaposlenja], [Zaposlen_do], [Lozinka], [Korisnicko_ime], [ID_Smjena], [ID_Rang]) VALUES (N'Pero', N'Peric', N'11111111111', CAST(N'2021-01-01' AS Date), NULL, N'Perke123', N'Perke', 1, 3)
INSERT [dbo].[Kontrolor] ([Ime], [Prezime], [OIB], [Datum_zaposlenja], [Zaposlen_do], [Lozinka], [Korisnicko_ime], [ID_Smjena], [ID_Rang]) VALUES (N'Stjepan', N'Štefan', N'12365478902', CAST(N'2019-10-09' AS Date), NULL, N'pogodiMojuLozinku123', N'stjepko', 1, 2)
INSERT [dbo].[Kontrolor] ([Ime], [Prezime], [OIB], [Datum_zaposlenja], [Zaposlen_do], [Lozinka], [Korisnicko_ime], [ID_Smjena], [ID_Rang]) VALUES (N'Živojin', N'Miric', N'33333333333', CAST(N'2003-03-03' AS Date), NULL, N'debata14', N'pogonskiEmzinjer', 1, 1)

GO

INSERT [dbo].[VoditeljSmjene] ([ID_Smjena]) VALUES (1)
GO

INSERT [dbo].[Status] ([Naziv_statusa]) VALUES (N'U tijeku')
INSERT [dbo].[Status] ([Naziv_statusa]) VALUES (N'Zavrsen')
GO


INSERT [dbo].[Prioritet] ([Stupanj_prioriteta]) VALUES (N'Najvisi')
INSERT [dbo].[Prioritet] ([Stupanj_prioriteta]) VALUES (N'Nizak')
INSERT [dbo].[Prioritet] ([Stupanj_prioriteta]) VALUES (N'Srednje')
INSERT [dbo].[Prioritet] ([Stupanj_prioriteta]) VALUES (N'Visok')
GO

INSERT [dbo].[NadzirePodsustav] ([ID_Podsustav], [ID_Kontrolor]) VALUES (1, 2)
GO
INSERT [dbo].[IzdajePrirucnik] ([ID_NadleznoTijelo], [ID_Prirucnik]) VALUES (1, 1)
INSERT [dbo].[IzdajePrirucnik] ([ID_NadleznoTijelo], [ID_Prirucnik]) VALUES (2, 1)
GO


INSERT [dbo].[PrimaInformaciju] ([ID_Kontrolor], [ID_Informacija]) VALUES (2, 1)
GO



INSERT [dbo].[RadniNalog] ([SLA], [Trajanje], [Tip_rada], [Trag_kvara], [Pocetak_rada], [ID_Kontrolor], [ID_VoditeljSmjene], [ID_Lokacija], [ID_Kvar], [ID_Status], [ID_StupanjPrioriteta]) VALUES (CAST(N'2021-02-03' AS Date), 2, N'Popravka', N'Klima uredaj je jucer cijeli dan hladio server', CAST(N'2021-02-02' AS Date), 1, 1, 1, 1, 2, 1)
INSERT [dbo].[RadniNalog] ([SLA], [Trajanje], [Tip_rada], [Trag_kvara], [Pocetak_rada], [ID_Kontrolor], [ID_VoditeljSmjene], [ID_Lokacija], [ID_Kvar], [ID_Status], [ID_StupanjPrioriteta]) VALUES (CAST(N'2021-06-06' AS Date), 20, N'Popravak', N'Anenometar krivo cita', CAST(N'2021-06-05' AS Date), 2, 1, 2, 2, 1, 2)

GO


INSERT [dbo].[RadniList] ([Pocetak_rada], [Trajanje_rada], [Opis_rada], [ID_Uredaj], [ID_TimZaOdrzavanje], [ID_RadniNalog], [ID_Status]) VALUES (CAST(N'2021-02-02' AS Date), 1, N'Provjera uticnice', 1, 1, 1, 2)
INSERT [dbo].[RadniList] ([Pocetak_rada], [Trajanje_rada], [Opis_rada], [ID_Uredaj], [ID_TimZaOdrzavanje], [ID_RadniNalog], [ID_Status]) VALUES (CAST(N'2021-06-05' AS Date), 10, N'Kalibriranje', 2, 2, 2, 1)

GO


