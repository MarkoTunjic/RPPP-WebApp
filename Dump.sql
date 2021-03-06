USE [master]
GO
USE [RPPP02]
GO
/****** Object:  Table [dbo].[Funkcije]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Funkcije](
	[Kategorija] [varchar](200) NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Naziv] [varchar](200) NOT NULL,
	[ID_Podsustav] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[ID_Podsustav] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Naziv] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Informacija]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Informacija](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Poruka] [varchar](200) NOT NULL,
	[Datum_slanja] [date] NOT NULL,
	[ID_Uredaj] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IzbioKvar]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IzbioKvar](
	[ID_Uredaj] [int] NOT NULL,
	[ID_Kvar] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID_Uredaj] ASC,
	[ID_Kvar] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IzdajePrirucnik]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IzdajePrirucnik](
	[ID_NadleznoTijelo] [int] NOT NULL,
	[ID_Prirucnik] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID_NadleznoTijelo] ASC,
	[ID_Prirucnik] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Kontrolor]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Kontrolor](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Ime] [varchar](100) NOT NULL,
	[Prezime] [varchar](200) NOT NULL,
	[OIB] [char](11) NOT NULL,
	[Datum_zaposlenja] [date] NOT NULL,
	[Zaposlen_do] [date] NULL,
	[Lozinka] [varchar](60) NOT NULL,
	[Korisnicko_ime] [varchar](100) NOT NULL,
	[ID_Smjena] [int] NOT NULL,
	[ID_Rang] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Korisnicko_ime] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[OIB] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[KrajSmjene]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KrajSmjene](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Vrijeme_kraja_smjene] [char](5) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Kriticnost]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Kriticnost](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Stupanj_kriticnosti] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Kvar]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Kvar](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Tip] [varchar](100) NOT NULL,
	[Ozbiljnost] [int] NOT NULL,
	[Opis] [varchar](200) NOT NULL,
	[ID_Prirucnik] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Lokacija]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Lokacija](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Naziv] [varchar](200) NOT NULL,
	[Postanski_broj] [char](5) NOT NULL,
	[Kontakt_telefon] [varchar](15) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NadleznoTijelo]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NadleznoTijelo](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Naziv] [varchar](200) NOT NULL,
	[Tip] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NadzirePodsustav]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NadzirePodsustav](
	[ID_Podsustav] [int] NOT NULL,
	[ID_Kontrolor] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID_Podsustav] ASC,
	[ID_Kontrolor] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NoviKvar]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NoviKvar](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Postupak_rjesavanja] [varchar](1862) NOT NULL,
	[Tip] [varchar](200) NOT NULL,
	[Ozbiljnost] [int] NOT NULL,
	[Opis] [varchar](200) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Oprema]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Oprema](
	[Redundantnost] [int] NOT NULL,
	[Budzet] [float] NOT NULL,
	[Datum_pustanja_u_pogon] [date] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_Podsustav] [int] NOT NULL,
	[ID_TipOpreme] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlaniranZa]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlaniranZa](
	[ID_Lokacija] [int] NOT NULL,
	[ID_PlanOdrzavanja] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID_Lokacija] ASC,
	[ID_PlanOdrzavanja] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlanOdrzavanja]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlanOdrzavanja](
	[Razina_strucnosti] [int] NOT NULL,
	[Datum_odrzavanja] [date] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_Podsustav] [int] NOT NULL,
	[ID_TimZaOdrzavanje] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PodrucjeRada]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PodrucjeRada](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Vrsta_podrucja_rada] [varchar](200) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Podsustav]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Podsustav](
	[Ucestalost_odrzavanja] [int] NOT NULL,
	[Osjetljivost] [int] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Naziv] [varchar](200) NOT NULL,
	[Opis] [varchar](200) NOT NULL,
	[ID_Sustav] [int] NOT NULL,
	[ID_Lokacija] [int] NOT NULL,
	[ID_Kriticnost] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Naziv] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PrimaInformaciju]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PrimaInformaciju](
	[ID_Kontrolor] [int] NOT NULL,
	[ID_Informacija] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID_Kontrolor] ASC,
	[ID_Informacija] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Prioritet]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Prioritet](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Stupanj_prioriteta] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Prirucnik]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Prirucnik](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Izdavac] [varchar](200) NOT NULL,
	[Verzija] [float] NOT NULL,
	[Datum] [date] NOT NULL,
	[Naziv] [varchar](200) NOT NULL,
	[ID_Uredaj] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Izdavac] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Naziv] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Verzija] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Radnik]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Radnik](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Ime] [varchar](100) NOT NULL,
	[Prezime] [varchar](100) NOT NULL,
	[Certifikat] [varchar](200) NULL,
	[Istek_certifikata] [date] NULL,
	[Dezuran] [int] NOT NULL,
	[ID_TimZaOdrzavanje] [int] NOT NULL,
	[ID_StrucnaSprema] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RadniList]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RadniList](
	[Pocetak_rada] [date] NOT NULL,
	[Trajanje_rada] [int] NOT NULL,
	[Opis_rada] [varchar](200) NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_Uredaj] [int] NOT NULL,
	[ID_TimZaOdrzavanje] [int] NOT NULL,
	[ID_RadniNalog] [int] NOT NULL,
	[ID_Status] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RadniNalog]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RadniNalog](
	[SLA] [date] NOT NULL,
	[Trajanje] [int] NOT NULL,
	[Tip_rada] [varchar](200) NOT NULL,
	[Trag_Kvara] [varchar](200) NOT NULL,
	[Pocetak_rada] [date] NOT NULL,
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_Kontrolor] [int] NOT NULL,
	[ID_VoditeljSmjene] [int] NOT NULL,
	[ID_Lokacija] [int] NOT NULL,
	[ID_Kvar] [int] NOT NULL,
	[ID_Status] [int] NOT NULL,
	[ID_StupanjPrioriteta] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Rang]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rang](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Ime_ranga] [varchar](200) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Smjena]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Smjena](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Pocetak_smjene] [char](5) NOT NULL,
	[Platni_faktor] [float] NOT NULL,
	[ID_KrajSmjene] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Stanje]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stanje](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Tip_stanja] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Status]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Status](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Naziv_statusa] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Stavka]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Stavka](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Koraci] [varchar](200) NOT NULL,
	[Namjena] [varchar](200) NOT NULL,
	[ID_Prirucnik] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StrucnaSprema]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StrucnaSprema](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Razina_strucne_spreme] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sustav]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sustav](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Osjetljivost] [int] NOT NULL,
	[Opis] [varchar](200) NOT NULL,
	[ID_Kriticnost] [int] NOT NULL,
	[ID_VrstaSustava] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TimZaOdrzavanje]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TimZaOdrzavanje](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Datum_osnivanja] [date] NOT NULL,
	[Naziv_tima] [varchar](200) NOT NULL,
	[Satnica] [int] NOT NULL,
	[ID_PodrucjeRada] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipOpreme]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipOpreme](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Tip_opreme] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UnosiSeKvar]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UnosiSeKvar](
	[ID_Prirucnik] [int] NOT NULL,
	[ID_NoviKvar] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID_Prirucnik] ASC,
	[ID_NoviKvar] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Uredaj]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Uredaj](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Naziv] [varchar](200) NOT NULL,
	[Proizvodac] [varchar](200) NOT NULL,
	[Godina_proizvodnje] [char](4) NOT NULL,
	[ID_Oprema] [int] NOT NULL,
	[ID_Stanje] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Naziv] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VoditeljSmjene]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VoditeljSmjene](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ID_Smjena] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VrstaSustava]    Script Date: 12/12/2021 05:21:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VrstaSustava](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Naziv_vrste_Sustava] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Funkcije]  WITH CHECK ADD FOREIGN KEY([ID_Podsustav])
REFERENCES [dbo].[Podsustav] ([ID])
GO
ALTER TABLE [dbo].[Informacija]  WITH CHECK ADD FOREIGN KEY([ID_Uredaj])
REFERENCES [dbo].[Uredaj] ([ID])
GO
ALTER TABLE [dbo].[IzbioKvar]  WITH CHECK ADD FOREIGN KEY([ID_Kvar])
REFERENCES [dbo].[Kvar] ([ID])
GO
ALTER TABLE [dbo].[IzbioKvar]  WITH CHECK ADD FOREIGN KEY([ID_Uredaj])
REFERENCES [dbo].[Uredaj] ([ID])
GO
ALTER TABLE [dbo].[IzdajePrirucnik]  WITH CHECK ADD FOREIGN KEY([ID_NadleznoTijelo])
REFERENCES [dbo].[NadleznoTijelo] ([ID])
GO
ALTER TABLE [dbo].[IzdajePrirucnik]  WITH CHECK ADD FOREIGN KEY([ID_Prirucnik])
REFERENCES [dbo].[Prirucnik] ([ID])
GO
ALTER TABLE [dbo].[Kontrolor]  WITH CHECK ADD  CONSTRAINT [FK__Kontrolor__ID_Ra__754E3E91] FOREIGN KEY([ID_Rang])
REFERENCES [dbo].[Rang] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Kontrolor] CHECK CONSTRAINT [FK__Kontrolor__ID_Ra__754E3E91]
GO
ALTER TABLE [dbo].[Kontrolor]  WITH CHECK ADD FOREIGN KEY([ID_Smjena])
REFERENCES [dbo].[Smjena] ([ID])
GO
ALTER TABLE [dbo].[Kvar]  WITH CHECK ADD FOREIGN KEY([ID_Prirucnik])
REFERENCES [dbo].[Prirucnik] ([ID])
GO
ALTER TABLE [dbo].[NadzirePodsustav]  WITH CHECK ADD FOREIGN KEY([ID_Kontrolor])
REFERENCES [dbo].[Kontrolor] ([ID])
GO
ALTER TABLE [dbo].[NadzirePodsustav]  WITH CHECK ADD FOREIGN KEY([ID_Podsustav])
REFERENCES [dbo].[Podsustav] ([ID])
GO
ALTER TABLE [dbo].[Oprema]  WITH CHECK ADD FOREIGN KEY([ID_Podsustav])
REFERENCES [dbo].[Podsustav] ([ID])
GO
ALTER TABLE [dbo].[Oprema]  WITH CHECK ADD FOREIGN KEY([ID_TipOpreme])
REFERENCES [dbo].[TipOpreme] ([ID])
GO
ALTER TABLE [dbo].[PlaniranZa]  WITH CHECK ADD FOREIGN KEY([ID_Lokacija])
REFERENCES [dbo].[Lokacija] ([ID])
GO
ALTER TABLE [dbo].[PlaniranZa]  WITH CHECK ADD  CONSTRAINT [FK__PlaniranZ__ID_Pl__04908221] FOREIGN KEY([ID_PlanOdrzavanja])
REFERENCES [dbo].[PlanOdrzavanja] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PlaniranZa] CHECK CONSTRAINT [FK__PlaniranZ__ID_Pl__04908221]
GO
ALTER TABLE [dbo].[PlanOdrzavanja]  WITH CHECK ADD FOREIGN KEY([ID_Podsustav])
REFERENCES [dbo].[Podsustav] ([ID])
GO
ALTER TABLE [dbo].[PlanOdrzavanja]  WITH CHECK ADD  CONSTRAINT [FK__PlanOdrza__ID_Ti__6517D6C8] FOREIGN KEY([ID_TimZaOdrzavanje])
REFERENCES [dbo].[TimZaOdrzavanje] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PlanOdrzavanja] CHECK CONSTRAINT [FK__PlanOdrza__ID_Ti__6517D6C8]
GO
ALTER TABLE [dbo].[Podsustav]  WITH CHECK ADD FOREIGN KEY([ID_Kriticnost])
REFERENCES [dbo].[Kriticnost] ([ID])
GO
ALTER TABLE [dbo].[Podsustav]  WITH CHECK ADD FOREIGN KEY([ID_Lokacija])
REFERENCES [dbo].[Lokacija] ([ID])
GO
ALTER TABLE [dbo].[Podsustav]  WITH CHECK ADD FOREIGN KEY([ID_Sustav])
REFERENCES [dbo].[Sustav] ([ID])
GO
ALTER TABLE [dbo].[PrimaInformaciju]  WITH CHECK ADD FOREIGN KEY([ID_Informacija])
REFERENCES [dbo].[Informacija] ([ID])
GO
ALTER TABLE [dbo].[PrimaInformaciju]  WITH CHECK ADD FOREIGN KEY([ID_Kontrolor])
REFERENCES [dbo].[Kontrolor] ([ID])
GO
ALTER TABLE [dbo].[Prirucnik]  WITH CHECK ADD FOREIGN KEY([ID_Uredaj])
REFERENCES [dbo].[Uredaj] ([ID])
GO
ALTER TABLE [dbo].[Radnik]  WITH CHECK ADD FOREIGN KEY([ID_StrucnaSprema])
REFERENCES [dbo].[StrucnaSprema] ([ID])
GO
ALTER TABLE [dbo].[Radnik]  WITH CHECK ADD  CONSTRAINT [FK__Radnik__ID_TimZa__59A6241C] FOREIGN KEY([ID_TimZaOdrzavanje])
REFERENCES [dbo].[TimZaOdrzavanje] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Radnik] CHECK CONSTRAINT [FK__Radnik__ID_TimZa__59A6241C]
GO
ALTER TABLE [dbo].[RadniList]  WITH CHECK ADD  CONSTRAINT [FK__RadniList__ID_Ra__24092D7A] FOREIGN KEY([ID_RadniNalog])
REFERENCES [dbo].[RadniNalog] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RadniList] CHECK CONSTRAINT [FK__RadniList__ID_Ra__24092D7A]
GO
ALTER TABLE [dbo].[RadniList]  WITH CHECK ADD FOREIGN KEY([ID_Status])
REFERENCES [dbo].[Status] ([ID])
GO
ALTER TABLE [dbo].[RadniList]  WITH CHECK ADD  CONSTRAINT [FK__RadniList__ID_Ti__23150941] FOREIGN KEY([ID_TimZaOdrzavanje])
REFERENCES [dbo].[TimZaOdrzavanje] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RadniList] CHECK CONSTRAINT [FK__RadniList__ID_Ti__23150941]
GO
ALTER TABLE [dbo].[RadniList]  WITH CHECK ADD FOREIGN KEY([ID_Uredaj])
REFERENCES [dbo].[Uredaj] ([ID])
GO
ALTER TABLE [dbo].[RadniNalog]  WITH CHECK ADD FOREIGN KEY([ID_Kontrolor])
REFERENCES [dbo].[Kontrolor] ([ID])
GO
ALTER TABLE [dbo].[RadniNalog]  WITH CHECK ADD FOREIGN KEY([ID_Kvar])
REFERENCES [dbo].[Kvar] ([ID])
GO
ALTER TABLE [dbo].[RadniNalog]  WITH CHECK ADD FOREIGN KEY([ID_Lokacija])
REFERENCES [dbo].[Lokacija] ([ID])
GO
ALTER TABLE [dbo].[RadniNalog]  WITH CHECK ADD  CONSTRAINT [FK__RadniNalo__ID_St__1E505424] FOREIGN KEY([ID_Status])
REFERENCES [dbo].[Status] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RadniNalog] CHECK CONSTRAINT [FK__RadniNalo__ID_St__1E505424]
GO
ALTER TABLE [dbo].[RadniNalog]  WITH CHECK ADD  CONSTRAINT [FK__RadniNalo__ID_St__1F44785D] FOREIGN KEY([ID_StupanjPrioriteta])
REFERENCES [dbo].[Prioritet] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RadniNalog] CHECK CONSTRAINT [FK__RadniNalo__ID_St__1F44785D]
GO
ALTER TABLE [dbo].[RadniNalog]  WITH CHECK ADD FOREIGN KEY([ID_VoditeljSmjene])
REFERENCES [dbo].[VoditeljSmjene] ([ID])
GO
ALTER TABLE [dbo].[Smjena]  WITH CHECK ADD FOREIGN KEY([ID_KrajSmjene])
REFERENCES [dbo].[KrajSmjene] ([ID])
GO
ALTER TABLE [dbo].[Stavka]  WITH CHECK ADD FOREIGN KEY([ID_Prirucnik])
REFERENCES [dbo].[Prirucnik] ([ID])
GO
ALTER TABLE [dbo].[Sustav]  WITH CHECK ADD FOREIGN KEY([ID_Kriticnost])
REFERENCES [dbo].[Kriticnost] ([ID])
GO
ALTER TABLE [dbo].[Sustav]  WITH CHECK ADD FOREIGN KEY([ID_VrstaSustava])
REFERENCES [dbo].[VrstaSustava] ([ID])
GO
ALTER TABLE [dbo].[TimZaOdrzavanje]  WITH CHECK ADD FOREIGN KEY([ID_PodrucjeRada])
REFERENCES [dbo].[PodrucjeRada] ([ID])
GO
ALTER TABLE [dbo].[UnosiSeKvar]  WITH CHECK ADD FOREIGN KEY([ID_NoviKvar])
REFERENCES [dbo].[NoviKvar] ([ID])
GO
ALTER TABLE [dbo].[UnosiSeKvar]  WITH CHECK ADD FOREIGN KEY([ID_Prirucnik])
REFERENCES [dbo].[Prirucnik] ([ID])
GO
ALTER TABLE [dbo].[Uredaj]  WITH CHECK ADD FOREIGN KEY([ID_Oprema])
REFERENCES [dbo].[Oprema] ([ID])
GO
ALTER TABLE [dbo].[Uredaj]  WITH CHECK ADD FOREIGN KEY([ID_Stanje])
REFERENCES [dbo].[Stanje] ([ID])
GO
ALTER TABLE [dbo].[VoditeljSmjene]  WITH CHECK ADD FOREIGN KEY([ID_Smjena])
REFERENCES [dbo].[Smjena] ([ID])
GO
USE [master]
GO
ALTER DATABASE [RPPP02] SET  READ_WRITE 
GO
