# RP3_Notepad
Projekt iz RP3

Text editor - aplikacija služi naravno za pisanje i uređivanje teksta. Treba biti omogućen rad s više dokumenata. Mora imati File i Edit izbornik (za učitavanje, spremanje datoteka, printanje, copy, paste, uređivanje fonta, postavljanje veličine uvlake (tab) i sl.). Neka je moguća i opcija da se u prozor editora uključi web pretraživač na mjesto po izboru (npr. u desni dio glavnog prozora), radi lakšeg kopiranja. Također, neka se može uključiti opcija da editor funkcionira djelomično kao C# editor. Konkretno, neka je pri upisivanju neke riječi uvijek ponuđen popis ključnih riječi ili varijabli koje su se dotad spomenule, a koje počinju s dotad napisanim slovima tako da korisnik može izabrati riječ iz izbornika. Neka su različitim bojama označeni svi parovi vitičastih zagrada i sl.

Done:
	- File izbornik
		- open, print, new (striktno navedeno u tekstu zadatka)
		- close (dodatno)
		- save (dodala sam da se file iskreira kad se pritisne 
		Save u SaveFileDialog-u, radi i replace ako file tog imena
		postoji)
		- dodala sam da kod odabira New kada otvori novi file i 
		prijeđe u taj tab, isto tako i kod open, ukoliko vam se 
		taj dio ne sviđa i želite da fokus ostane na tabu u kojem smo bili
		treba u metodama za te dvije opcije samo obrisati liniju: 
		"tabControl1.SelectTab(tp);"
	- Edit izbornik
		- copy, paste (striktno navedeno u tekstu zadatka)
		- cut, select all, delete (dodatno)
		- change font (size, color, style)
	-Web browser(Morao sam sve staviti u split container,pa se zbog tehnickih razloga sad kontrola TabControl zove
	 tabControl2 a ne tabControl1.)
	-Perina vecera,inace jako ukusna je bila
	
	
Not done (a navedeno u zad je pa je must):
	- Edit izbornik
		- tab size
	- Dodatne opcije
		- c# editor
		-nadodati scroll za web browser ako se može, mada ja nisam uspio u split containeru,
		 ili skalirati browser(ovo skaliranje djeluje nemoguce)
	
	
Tabove najbolje kao i ostalo sa tekstom samo zabranit default opciju (što god da radi tipka) i onda nadodat naše (Zvone)
