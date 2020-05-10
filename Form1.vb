Imports Microsoft.Office.Interop.Excel
Imports System.Configuration
Imports System.Collections.Specialized


Public Class Form1

#Region "Degisken Tanımlama Bölgesi"





    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Flowtext.Text = "1600"
        CODtext.Text = "10000"
        TKNgiriştext.Text = "2800"
        temptext.Text = "25"
        TNçıkıştext.Text = "30"
        NH4cikistext.Text = "1"
        MLSStesisiçitext.Text = "9000"
        Q_Qrtext.Text = "7.3"
        dekantörçıkıştext.Text = "25"
        disoxygentext.Text = "1.5"
        TSSgiriştext.Text = "1100"
        tesisrakimtext.Text = "200"
        tankdepthtext.Text = "4.5"

    End Sub





    Private Sub temizlebtn_Click(sender As Object, e As EventArgs) Handles temizlebtn.Click
        'temizleme kodu
        camuryasitext.Clear()
        fazlacamurtext.Clear()
        aerobiktanktext.Clear()
        Anaerobiktantext.Clear()
        MLVSStext.Clear()
        İcgeridevirtext.Clear()
        QIRtext.Clear()
        SDNRtext.Clear()
        NO3cikistext.Clear()
        oksijenihtiyacitext.Clear()
        sotrtext.Clear()
        blowerhavatext.Clear()
        Vratiotext.Clear()
        metanoltext.Clear()
        yenimlsstext.Clear()


    End Sub






    Dim Flow, yukseklikvlook, Temp, COD, TKNgiris, NH4giris, TPgiris, MLSStesisici, TNcikis, disoxygen, TSSgiris As Double
    Dim BODgiris, sBOD, sCOD, rbCOD, VSSgiris, difuzorverim, Tankdepth, rakim As Double
    Dim metanol As Double
    Dim SDNRbirim As Double
    Dim NOxcikishesap As Double
    Dim o2kazanci, neto2ihtiyaci As Double
    Dim bCOD, nbCOD, nbsCODeff, nbpCOD, VSScod, nbVSS, iTSS, NOx As Double
    Dim TCnumaxAOB, numaxAOB, TCbAOB, bAOB, SNH4, nuAOB, KNH4, KoAOB, TeorikSRT, TasarimSRT, SF, bH, TCbH As Double
    Dim numax, TCnumax, Ks, S, Pxbiokabul, Y, fd, Yn As Double
    Dim flowtoanoksiktank As Double
    Dim A, B, C0 As Double
    Dim NOxdogru, Pxbio, C, Pxvss, Pxtss, Voksik, HRT, MLVSS As Double
    Dim Xb, NH4cikis, Ne, IR, R, NOxfeed, Tankhacimorani, Vanoksik, F_Mb As Double
    Dim rbCODfraksiyonu, b0, b1, SDNRb, SDNRt, SDNRadj, SDNR, NOr, OTR As Double
    Dim gravity, za, zb, Rsabiti, mol, Pb_Pa As Double
    Dim Cs20, Cssonsuz20, De, Df, Pa, alfa, beta, Fouling As Double
    Dim Cst, SOTR, SOTE, O2conc, Havadebisi As Double
    Private Sub metanolbtn_Click(sender As Object, e As EventArgs) Handles metanolbtn.Click

        Do Until NOxcikishesap < Ne
            metanol = 0
            metanol = metanol + 2100

            COD = COD + ((metanol / Flow) * 1500)
            Dim bodSabiti As String = ConfigurationManager.AppSettings.Get("BOD-COD-YaklasimSabiti")
            BODgiris = Convert.ToDouble(bodSabiti) * COD  ' bu bir yaklaşımdır değişebilir ref. Barış hoca  0.55 "
            sBOD = BODgiris * 0.95
            sCOD = 0.95 * COD
            rbCOD = 0.45 * sCOD
            TSSgiris = CDbl(TSSgiriştext.Text)
            VSSgiris = TSSgiris * 0.3
            Temp = CDbl(temptext.Text)
            TKNgiris = CDbl(TKNgiriştext.Text)
            VSSgiriştext.Text = VSSgiris

            NH4giris = TKNgiris * 0.85
            TNcikis = CDbl(TNçıkıştext.Text)
            koitknratiotext.Text = CDbl(CODtext.Text) / TKNgiris
            koitknratiotext.Text = Math.Round(Val(koitknratiotext.Text), 1)
            NH4cikis = CDbl(NH4cikistext.Text)
            Ne = TNcikis - NH4cikis

            NOxcikistext.Text = Ne

            NOx = TKNgiris * 0.8  ' burası sonradan doğrulanacak

            '1) Gerekli giriş parametrelerinin hesaplanması

            'a. bCOD değerinin Hesaplanması

            bCOD = 1.6 * BODgiris 'bCOD = 1.6 * BODgiriş  "metcalf eddy"

            'b. nbCOD değerinin Hesaplanması

            nbCOD = COD - bCOD
            'c. nbsKOİç değerinin Hesaplanması
            nbsCODeff = sCOD - 1.6 * sBOD

            'd. nbpKOİ, UAKMKOİ, nbUAKM değerlerinin Hesaplanması

            nbpCOD = COD - bCOD - nbsCODeff

            VSScod = (COD - sCOD) / VSSgiris

            nbVSS = nbpCOD / VSScod

            'e. İnert AKM değerinin hesaplanması

            iTSS = TSSgiris - VSSgiris


            'bKOİ : Biyolojik olarak parçalanabilen KOİ, mg/L
            'nbKOİ : Biyolojik olarak parçalanamayan KOİ, mg/L
            'nbsKOİç : Çözünmüş haldeki biyolojik olarak parçalanamayan KOİ çıkış, mg/L
            'nbpKOİ : Partikül halindeki biyolojik olarak parçalanamayan KOİ, mg/L
            'nbUAKM : Biyolojik olarak parçalanmayan uçucu askıda katı madde, mg/L


            '2)Nitrifikasyon bakterileri için spesifik büyüme hızının hesaplanması

            'a. T= 25 C de nümaxaob değerinin hesaplanması

            numaxAOB = 0.9 'g/g.gün ' NH4 oksidasyonu için gerekli değer 'Metcalf eddy tablo 8-14

            TCnumaxAOB = numaxAOB * (1.072) ^ (Temp - 20) ' TC temp. correction anlamına gelmektedir.

            'b. T= 25 bAOB değerinin hesaplanması

            bAOB = 0.17  'g/g.gün

            TCbAOB = bAOB * (1.029 ^ (Temp - 20))

            'c.nüAOB değerinin hesaplanması

            SNH4 = NH4cikis ' çıkış NH+-N miktarı assume edilmiştir
            KNH4 = 0.5 'mg/L ' Yarılanma hız sabiti, 'Metcalf eddy tablo 8-14
            KoAOB = 0.5 'Çözünmüş oksijen için yarılanma hız sabiti ''Metcalf eddy tablo 8-14

            disoxygen = CDbl(disoxygentext.Text)
            nuAOB = TCnumaxAOB * (SNH4 / (SNH4 + KNH4)) * (disoxygen / (disoxygen + KoAOB)) - TCbAOB


            'nüAOB : Spesifik buyume hız, g UAKM/g UAKM.gun
            'nümax, AOB : Maksimum spesifik buyume h.z., g UAKM/g UAKM.gun
            'SNH4 :  çıkış amonyum konsantrasyonu, mg/L
            'KNH4 : Yarılanma hız sabiti, mg/L
            'Disoxygen : Oksik (aerobik) tankta istenilen cozunmu. oksijen konsantrasyonu, mg/L
            'bAOB : Nitrifikasyon bakterisi icin bozunma katsay.s., gun-1
            'KoAOB =Çözünmüş oksijen için yarılanma hız sabiti 'Metcalf eddy tablo 8-14


            '3) Teorik ve tasarım çamur yaşının (SRT) hesaplanamsı


            TeorikSRT = 1 / nuAOB

            SF = 2 'emniyet faktörüdür değiştirilebilir

            TasarimSRT = SF * (TeorikSRT)

            camuryasitext.Text = TasarimSRT
            camuryasitext.Text = Math.Round(Val(camuryasitext.Text), 1)
            '4) Biyolojik arıtma ünitelerinden çıkan fazla çamur miktarı hesaplanması

            'a. Çıkış bKOİ (S) değerinin hesaplanması

            'Formüllerde kullanılan olan Ks, bH, fd, Y değerleri e Metcalf eddy tablo 8-14 ten alınmıştır.

            'Temp.correction bH

            bH = 0.17
            TCbH = bH * ((1.04) ^ (Temp - 20))


            'Temp.correction nümax

            numax = 0.9  'Maksimum spesifik buyume h.z., g UAKM/g UAKM.gun  ' 2. bölümde kullanılan nümax,aob ile aynı parametre

            TCnumax = numax * ((1.07) ^ (Temp - 20))


            'çıkış bKOİ(S) hesabı
            'S = çıkış bKOİ

            Ks = 8 'Metcalf eddy tablo 8-14
            S = Ks * (1 + (TCbH * TasarimSRT)) / (TasarimSRT * (TCnumax - TCbH) - 1)


            'b. Fazla çamur (Px) miktarının hesaplanması

            'NOx : Nitrifikasyon bakteri biyokütlesi hesabında gerekli nitrata dönüşen amonyum azotu 'KABULLER bölümünde açıklanmıştır.

            Y = 0.45  'Metcalf eddy tablo 8-14
            Yn = 0.15 'Metcalf eddy tablo 8-14 ' nitrifikasyon bakterisi için gerekli verim sabiti
            fd = 0.15  'Metcalf eddy tablo 8-14

            'A=heteretrofik biyokütleden kaynaklı oluşan çamur
            'B=hücre yıkıntısından kaynaklı çamur
            'C=nitrifikasyon bakterisisinden kaynaklı çamur
            'A+B+C0 = TOPLAM ÇAMUR, Pxbiokabul

            A = (Flow * Y * (bCOD - S) / 1000) / (1 + TCbH * (TasarimSRT))

            B = ((fd * TCbH * Y * Flow * (bCOD - S) * TasarimSRT) / 1000 / (1 + TCbH * TasarimSRT))

            C0 = ((Flow * Yn * NOx) / 1000) / (1 + TCbAOB * TasarimSRT)

            Pxbiokabul = A + B + C0

            'PX,bio : Biyolojik fazla çamur kg/gün ' İçerisinde heteretrofik bakteri biyokütlesi, hücre yıkıntısı, nitrifikasyon bakteri biyokütlesi var
            'SRT:  Çamur yaşı, gün
            'bH : Heteretrofik bakteri için ozunma katsayısı, gün-1
            'bAOB : Nitrifikasyon bakterisi için için bozunma katsayısı, gün-1
            'Y : Verim sabiti (Dönüşüm oranı) g UAKM/g okside substrat
            'fd : Hücre yıkıntısı olarak kalan hücre kütle fraksiyonu, g/g
            'NOx : Nitrata dönüşmüş amonyum azotu, mg/L
            'S0 : Giriş bKOİ konsantrasyonu, mg/L
            'S:  Çıkış bKOİ, konsantrasyonu, mg / L


            '5) Nitrata dönüşen amonyum azotu miktarının hesaplanarak doğrulanması



            'NOx mikarının doğrulanması normalde (TKN * %80) olarak kabul edilmişti


            NOxdogru = TKNgiris - TNcikis - (0.12 * Pxbiokabul * 1000 / Flow)


            'yeni NOx değeri ile PxBio miktarı da tekrar hesaplanmalıdır.


            C = ((Flow * Yn * NOxdogru) / 1000) / (1 + TCbAOB * TasarimSRT)

            Pxbio = A + B + C
            '6) Fazla çamur miktarının hesaplanması


            'a. PXvss yükü miktarı

            Pxvss = Pxbio + (Flow * nbVSS / 1000)

            'b. PXtss yükü miktarı

            Pxtss = (Pxbio / 0.85) + (Flow * nbVSS / 1000) + Flow * iTSS / 1000
            fazlacamurtext.Text = Pxtss
            fazlacamurtext.Text = Math.Round(Val(fazlacamurtext.Text), 1)


            '7) Havalandırma (Nitrifikasyon) tank hacmi ve hidrolik bekletme süresinin hesaplanması

            'a. Tank hacminin hesaplanması

            If metanol = 0 Then
                MLSStesisici = CDbl(MLSStesisiçitext.Text)
            Else
                MLSStesisici = (Pxtss * TasarimSRT) / Voksik * 1000
            End If


            Voksik = (TasarimSRT * Pxtss / MLSStesisici) * 1000

            aerobiktanktext.Text = Voksik
            aerobiktanktext.Text = Math.Round(Val(aerobiktanktext.Text), 1)

            'b. Hidrolik bekletme süresinin hesaplanması

            HRT = Voksik * 24 / Flow  '24 ile saat/gün çevirimi yapılmıştır.



            'c. MLVSS miktarının hesaplanması

            MLVSS = (Pxvss / Pxtss) * MLSStesisici

            MLVSStext.Text = MLVSS
            MLVSStext.Text = Math.Round(Val(MLVSStext.Text), 1)

            'C. ANOKSİK TANK (DENİTRİFİKASYON) HESAPLARI

            '1) Aktif biyokütkle konsantrasyonunun hesaplanması

            Xb = ((Flow * TasarimSRT) / Voksik) * ((Y * (bCOD - S)) / (1 + TCbH * TasarimSRT))

            'Xb : Tanktaki Aktif biyokütle, mg/L
            'YH : Verim sabiti (Dönüşüm oranı) g UAKM/g okside substrat
            'bCOD (S0) : Giriş bKOİ konsantrasyonu , mg/L
            'S : Çıkış bKOİ konsantrasyonu , mg/L
            'TCbH : Heteretrofik bakteriler için bozunma katsayısı, gün-1, sıcaklık doğrulaması yapılmış
            'TasarımSRT : Nitrifikasyon için hesaplanan çamur yaşı, gün


            '2) İç geri devir(IR) oranının hesaplanması

            'NH4cikis = Sayfa3.Range("K13")  'tesisin sonundaki çıkış amonyum miktarı

            Ne = TNcikis - NH4cikis   'Ne: çıkışta istenilen nitrat miktarı

            ' R = Sayfa3.Range("G241")
            R = CDbl(Q_Qrtext.Text)
            IR = (NOxdogru / Ne) - 1 - R

            İcgeridevirtext.Text = IR
            İcgeridevirtext.Text = Math.Round(Val(İcgeridevirtext.Text), 1)

            'IR : İç geri devir oranı
            'NOx : Oksik tankta nitrifiye edilen amonyum azotu, mgNO3-N/L
            'Ne : Çıkış NO3-N konsantrasyonu, mg/L
            'TNçıkış : Toplam Azot çıkış konsantrasyonu, mg/L
            'NH4çkış : Çıkış Amonyum konsantrasyonu, mg/L
            'R : Çamur geri devir oranı, RAS ratio


            '3) Oksik tanktan anoksik tanka geçen nitrat yükünün hesaplanması


            flowtoanoksiktank = (IR * Flow + R * Flow) / 24
            QIRtext.Text = flowtoanoksiktank
            QIRtext.Text = Math.Round(Val(QIRtext.Text), 1)

            NOxfeed = (IR * Flow + R * Flow) * Ne / 1000



            'NOx (feed) : Oksik tanktan anoksik tanka beslenen Nitrat miktarı, kg/gün



            '4) Denitrifikasyon için gerekli anoksik tank hacminin hesaplanması
            Dim noxnew As Double

            noxnew = 100000
don:
            Vanoksik = Tankhacimorani * Voksik
            Anaerobiktantext.Text = Vanoksik
            Anaerobiktantext.Text = Math.Round(Val(Anaerobiktantext.Text), 1)


            '5) F/Mb oranının belirlenmesi

            F_Mb = Flow * BODgiris / (Vanoksik * Xb)


            'F/Mb : Besi maddesi mikroorganizma oranı, gBOD/g biyokütle.gün
            'Xb : Anoksik bölge biyokütle konsantrasyonu, mg/L
            'Vnox : Anoksik tank hacmi, m3

            '6) Standart Denitrifikasyon hızının (SDNR) hesaplanması


            If F_Mb > 0.5 And F_Mb < 1 Then


                rbCODfraksiyonu = (rbCOD / bCOD) * 100

                If rbCODfraksiyonu > 4.5 And rbCODfraksiyonu < 14.5 Then

                    b0 = 0.186
                    b1 = 0.078

                ElseIf rbCODfraksiyonu > 14.5 And rbCODfraksiyonu < 24.5 Then


                    b0 = 0.213
                    b1 = 0.118

                ElseIf rbCODfraksiyonu > 24.5 And rbCODfraksiyonu < 34.5 Then

                    b0 = 0.235
                    b1 = 0.141

                ElseIf rbCODfraksiyonu > 34.5 And rbCODfraksiyonu < 44.5 Then

                    b0 = 0.242
                    b1 = 0.152

                ElseIf rbCODfraksiyonu > 44.5 Then

                    b0 = 0.27
                    b1 = 0.162

                End If

                SDNRb = b0 + b1 * Math.Log(F_Mb)
                Dim ff As Double = Math.Exp(F_Mb)
                SDNRt = SDNRb * (1.026 ^ (Temp - 20))

            ElseIf F_Mb < 0.5 Then

                SDNRb = 0.24 * F_Mb
                SDNRt = SDNRb * (1.026 ^ (Temp - 20))
            ElseIf F_Mb >= 1 Then

                rbCODfraksiyonu = (rbCOD / bCOD) * 100


                If rbCODfraksiyonu > 4.5 And rbCODfraksiyonu < 14.5 Then

                    b0 = 0.186
                    b1 = 0.078

                ElseIf rbCODfraksiyonu > 14.5 And rbCODfraksiyonu < 24.5 Then


                    b0 = 0.213
                    b1 = 0.118

                ElseIf rbCODfraksiyonu > 24.5 And rbCODfraksiyonu < 34.5 Then


                    b0 = 0.235
                    b1 = 0.141

                ElseIf rbCODfraksiyonu > 34.5 And rbCODfraksiyonu < 44.5 Then

                    b0 = 0.242
                    b1 = 0.152

                ElseIf rbCODfraksiyonu > 44.5 Then


                    b0 = 0.27
                    b1 = 0.162

                End If

                SDNRb = b0 + b1 * Math.Log(F_Mb)

                SDNRt = SDNRb * (1.026 ^ (Temp - 20))

                SDNRadj = SDNRt - (0.029 * Math.Log(F_Mb)) - 0.012

                'SDNRt: Bu hesap eğer F/Mb oranı 1 den büyükse uygulanıyor değilse uygulamaya gerek yok, sıcaklık doğrulaması
                'SDNRadj:Bu hesap da eğer F/Mb oranı 1 den büyükse uygulanıyor, IR doğrulaması



                SDNRt = SDNRadj ' burada aşağıdaki formülde SDNRt kullanıldığı için SDNRadj olarak tanımlandı


            End If

            '7) SDNR değerinin aktif biyokütleye (Xb) göre hesaplanması OVERALL SDNR

            SDNR = SDNRt * Xb / MLVSS
            SDNRbirim = SDNR * 1000 / 24

            SDNRtext.Text = SDNRbirim
            SDNRtext.Text = Math.Round(Val(SDNRtext.Text), 1)



            'SDNR: Bu değer Metcalf eddy'de hesaplanmaktadır fakat kullanılmamamktadır.
            '8) Giderilmesi Hesaplanan potansiyel Nitrat yükünün hesaplanması

            NOr = Vanoksik * SDNRt * Xb / 1000

            'NOr: Hesaplara göre giderilebilecek nitrat yükü, g/gün


            'NOx çıkış niktarının hesabı





            NOxcikishesap = ((NOxfeed - NOr) * 1000) / Flow

            NO3cikistext.Text = NOxcikishesap
            NO3cikistext.Text = Math.Round(Val(NO3cikistext.Text), 1)



            ''' Bu döngü hacim oranını hesaplama döngüsüdür 

            Do Until NOxcikishesap > noxnew

                noxnew = NOxcikishesap

                Tankhacimorani = Tankhacimorani + 0.1

                GoTo don

            Loop

            Vratiotext.Text = Tankhacimorani - 0.1
            Tankhacimorani = CDbl(Vratiotext.Text)
            ''BURADA YUKARIDAKİİ KODLARIN BAZILARI TEKRAR ALINMIŞTIR ... üst sınır

            tankhacimdogrulamakodu() ' burada doğru tank hacminin tekrar koda girmesi sağlanıyor

            '9) Biyolojik arıtma için gerekli net oksijen miktarının hesaplanması


            OTR = ((Flow * (bCOD - S)) / 1000 - (1.42 * (A + B)) + (4.57 * NOxdogru * Flow) / 1000 - (2.86 * (NOxdogru - Ne) * Flow) / 1000) / 24



            oksijenihtiyacitext.Text = OTR
            oksijenihtiyacitext.Text = Math.Round(Val(oksijenihtiyacitext.Text), 1)

            'OTR : Gerçek oksijen ihtiyacı, kgO2/saat
            'bCOD : Giriş bKOİ konsantrasyonu , mg/L
            'S : Çıkış bKOİ konsantrasyonu , mg/L
            'Px , bioVSS: Fazla Çamur, kg / gün, (nitrifikasyon bakterileri hesaba katılmadı), O yüzden (A+B) olarak yazıldı
            'NN : Nitrifiye olacak amonyum azotu, kg/gün
            'NDN : Denitrifiye olacak nitrat azotu, kg/gün


            'SOTR hesabı için gerekli küçük hesaplamalar

            gravity = 9.81            'm/s^2
            rakim = CDbl(tesisrakimtext.Text)  'tesis rakım
            za = 0              'deniz seviyesi
            Rsabiti = 8314            '(kg*m^2)/(s^2*kg*K*mol)
            mol = 28.97         'kg/kg.mol
            Pb_Pa = Math.Exp(-1 * (gravity * mol * (rakim - za)) / (Rsabiti * (273.15 + Temp)))



            'Pb/Pa : H rakımındaki bağıl basınç oranı
            'gravity:  Yerçekimi kuvveti, M / s2
            'Rsabiti : Evrensel gaz sabiti, mol hava.Kelvin
            'M : Havanın molekül ağırlığı, g/g.mol, (28.94 g/g.mol)
            'rakım-za : Yükseklik farkı, m, (200m)
            'Pa : Deniz seviyesinde atmosfer basıncı, atm
            'Pb : H yüksekliğindeki atmosfer basıncı, atm
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

            Cs20 = 9.09 '20 C ve 1 atm’de temiz suda çözünmüş oksijen doygunluk konsantrasyonu , Metcalf eddy appandix E-1

            De = 0.4 'Orta su seviyesi doğrulama katsayısı, tipik değeri 0,25-0,45
            Df = 0.5 'Difüzör derinliği
            Pa = 10.33 'Deniz seviyesinde atmosfer basıncı, atm
            Tankdepth = CDbl(tankdepthtext.Text)
            Cssonsuz20 = Cs20 * (1 + De * (Tankdepth - Df) / Pa)


            'Df: Difüzör yüksekliği, formülde bu değer tank yüksekliğinden çıkarılır
            'De : Orta su seviyesi doğrulama katsayısı, tipik değeri 0,25-0,45
            'Cssonsuz20: 20C sıcaklıkta ve H rakımında havalandırma tankında temiz sudaki çözünmüş oksijen ortalama doygunluk konsantrasyonu, mg/L."







            'alfa hesabı pöpel formülü

            alfa = 1 - (0.16 * (MLSStesisici / 1000) ^ (2 / 3))

            If rakim > 0 And rakim < 50 Then
                yukseklikvlook = 2
            ElseIf rakim > 50 And rakim < 300 Then
                yukseklikvlook = 3
            ElseIf rakim > 300 And rakim < 500 Then
                yukseklikvlook = 4
            ElseIf rakim > 500 And rakim < 700 Then
                yukseklikvlook = 5
            ElseIf rakim > 700 And rakim < 900 Then
                yukseklikvlook = 6
            ElseIf rakim > 900 And rakim < 1100 Then
                yukseklikvlook = 7
            ElseIf rakim > 1100 And rakim < 1300 Then
                yukseklikvlook = 8
            ElseIf rakim > 1300 And rakim < 1500 Then
                yukseklikvlook = 9
            ElseIf rakim > 1500 And rakim < 1700 Then
                yukseklikvlook = 10
            ElseIf rakim > 1700 And rakim < 1900 Then
                yukseklikvlook = 11

            End If

            'Dim excelTabloVerisi As Double = ExcelHelper.ExcelDosyaDizinindenOku(1, "A")
            'Dim excelTabloVerisi As Double = ExcelHelper.ExcelDosyaDizinindenOku(Temp, yukseklikvlook)  'ASIL KOD BU

            'Dim arananDeger As Double = ExcelApplication.WorksheetFunction.VLookup(25, worksheetSayfa1.Range("B39:L79"), 2, False)
            'Cst = Application.WorksheetFunction.VLookup(Temp, Sayfa2.Range("B39:L79"), yukseklikvlook, False)


            Cst = 8.063 'excelTabloVerisi  BAŞTAKİ 8.063 Ü SİL 

            Fouling = 0.9 'Tıkanma faktörü, tipik değeri 0.65- 0,9

            beta = 0.95  'Tuzluluk – yüzey gerilimi düzeltme faktörü, genellikle 0,95

            SOTR = (OTR / (alfa * Fouling)) * (Cssonsuz20 / (((beta * (Cst / Cs20) * Pb_Pa * Cssonsuz20 - disoxygen)))) * (1.024 ^ (20 - Temp))
            sotrtext.Text = SOTR
            sotrtext.Text = Math.Round(Val(sotrtext.Text), 1)

            'SOTR:Standart oksijen ihtiyacı, kg/saat

            '10) Biyolojik arıtma için gerekli hava miktarının hesaplanması


            SOTE = 28 ' BU DEĞER DE DEĞİŞEBİLİR

            'SOTE: Difüzör verimi, tesiste bulunan difüzörlere göre hesaplandı

            O2conc = 0.276   'kg O2/m3 ' metcalf eddy sayfa 1910 Appendix b-3 'barış hocayla hesaplayıp karar vermiştik alttaki 3 satırda açıklaması var ama 12 C ye ait

            '12oC ve belirtilen hava basıncında havanın yogunlugu 1.1633 kg/m3 alınabilir.
            'Belirtilen kosullarda hava içersindeki oksijenin ağırlıkça oranının 0,23 oldugu kabul edilirse;
            'hava içerisindeki oksijen konsantrasyonu; 0,23x1,1633 kg/m3 = 0,270  hava bulunacaktır.

            Havadebisi = SOTR / ((SOTE / 100) * O2conc) 'm^3 hava /saat
            blowerhavatext.Text = Havadebisi
            blowerhavatext.Text = Math.Round(Val(blowerhavatext.Text), 1)


            metanoltext.Text = metanol
            yenimlsstext.Text = MLSStesisici
            yenimlsstext.Text = Math.Round(Val(yenimlsstext.Text), 1)

        Loop

    End Sub

#End Region



    ''' <summary>
    ''' Hesapla metodu.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub btnHesapla_Click(sender As Object, e As EventArgs) Handles btnHesapla.Click

        Dim bodSabiti As String = ConfigurationManager.AppSettings.Get("BOD-COD-YaklasimSabiti")
        Flow = CDbl(Flowtext.Text)

        COD = CDbl(CODtext.Text)
        Tankhacimorani = 1  'başlangıç olarak optimum oran 1 olarak belirlenmiştir.
CODhesabi:
        BODgiris = Convert.ToDouble(bodSabiti) * COD  ' bu bir yaklaşımdır değişebilir ref. Barış hoca  0.55 "
        sBOD = BODgiris * 0.95
        sCOD = 0.95 * COD
        rbCOD = 0.45 * sCOD
        TSSgiris = CDbl(TSSgiriştext.Text)
        VSSgiris = TSSgiris * 0.3
        Temp = CDbl(temptext.Text)
        TKNgiris = CDbl(TKNgiriştext.Text)
        VSSgiriştext.Text = VSSgiris

        NH4giris = TKNgiris * 0.85
        TNcikis = CDbl(TNçıkıştext.Text)
        koitknratiotext.Text = CDbl(CODtext.Text) / TKNgiris
        koitknratiotext.Text = Math.Round(Val(koitknratiotext.Text), 1)
        NH4cikis = CDbl(NH4cikistext.Text)
        Ne = TNcikis - NH4cikis

        NOxcikistext.Text = Ne

        NOx = TKNgiris * 0.8  ' burası sonradan doğrulanacak

        '1) Gerekli giriş parametrelerinin hesaplanması

        'a. bCOD değerinin Hesaplanması

        bCOD = 1.6 * BODgiris 'bCOD = 1.6 * BODgiriş  "metcalf eddy"

        'b. nbCOD değerinin Hesaplanması

        nbCOD = COD - bCOD
        'c. nbsKOİç değerinin Hesaplanması
        nbsCODeff = sCOD - 1.6 * sBOD

        'd. nbpKOİ, UAKMKOİ, nbUAKM değerlerinin Hesaplanması

        nbpCOD = COD - bCOD - nbsCODeff

        VSScod = (COD - sCOD) / VSSgiris

        nbVSS = nbpCOD / VSScod

        'e. İnert AKM değerinin hesaplanması

        iTSS = TSSgiris - VSSgiris


        'bKOİ : Biyolojik olarak parçalanabilen KOİ, mg/L
        'nbKOİ : Biyolojik olarak parçalanamayan KOİ, mg/L
        'nbsKOİç : Çözünmüş haldeki biyolojik olarak parçalanamayan KOİ çıkış, mg/L
        'nbpKOİ : Partikül halindeki biyolojik olarak parçalanamayan KOİ, mg/L
        'nbUAKM : Biyolojik olarak parçalanmayan uçucu askıda katı madde, mg/L


        '2)Nitrifikasyon bakterileri için spesifik büyüme hızının hesaplanması

        'a. T= 25 C de nümaxaob değerinin hesaplanması

        numaxAOB = 0.9 'g/g.gün ' NH4 oksidasyonu için gerekli değer 'Metcalf eddy tablo 8-14

        TCnumaxAOB = numaxAOB * (1.072) ^ (Temp - 20) ' TC temp. correction anlamına gelmektedir.

        'b. T= 25 bAOB değerinin hesaplanması

        bAOB = 0.17  'g/g.gün

        TCbAOB = bAOB * (1.029 ^ (Temp - 20))

        'c.nüAOB değerinin hesaplanması

        SNH4 = NH4cikis ' çıkış NH+-N miktarı assume edilmiştir
        KNH4 = 0.5 'mg/L ' Yarılanma hız sabiti, 'Metcalf eddy tablo 8-14
        KoAOB = 0.5 'Çözünmüş oksijen için yarılanma hız sabiti ''Metcalf eddy tablo 8-14

        disoxygen = CDbl(disoxygentext.Text)
        nuAOB = TCnumaxAOB * (SNH4 / (SNH4 + KNH4)) * (disoxygen / (disoxygen + KoAOB)) - TCbAOB


        'nüAOB : Spesifik buyume hız, g UAKM/g UAKM.gun
        'nümax, AOB : Maksimum spesifik buyume h.z., g UAKM/g UAKM.gun
        'SNH4 :  çıkış amonyum konsantrasyonu, mg/L
        'KNH4 : Yarılanma hız sabiti, mg/L
        'Disoxygen : Oksik (aerobik) tankta istenilen cozunmu. oksijen konsantrasyonu, mg/L
        'bAOB : Nitrifikasyon bakterisi icin bozunma katsay.s., gun-1
        'KoAOB =Çözünmüş oksijen için yarılanma hız sabiti 'Metcalf eddy tablo 8-14


        '3) Teorik ve tasarım çamur yaşının (SRT) hesaplanamsı


        TeorikSRT = 1 / nuAOB

        SF = 2 'emniyet faktörüdür değiştirilebilir

        TasarimSRT = SF * (TeorikSRT)

        camuryasitext.Text = TasarimSRT
        camuryasitext.Text = Math.Round(Val(camuryasitext.Text), 1)
        '4) Biyolojik arıtma ünitelerinden çıkan fazla çamur miktarı hesaplanması

        'a. Çıkış bKOİ (S) değerinin hesaplanması

        'Formüllerde kullanılan olan Ks, bH, fd, Y değerleri e Metcalf eddy tablo 8-14 ten alınmıştır.

        'Temp.correction bH

        bH = 0.17
        TCbH = bH * ((1.04) ^ (Temp - 20))


        'Temp.correction nümax

        numax = 0.9  'Maksimum spesifik buyume h.z., g UAKM/g UAKM.gun  ' 2. bölümde kullanılan nümax,aob ile aynı parametre

        TCnumax = numax * ((1.07) ^ (Temp - 20))


        'çıkış bKOİ(S) hesabı
        'S = çıkış bKOİ

        Ks = 8 'Metcalf eddy tablo 8-14
        S = Ks * (1 + (TCbH * TasarimSRT)) / (TasarimSRT * (TCnumax - TCbH) - 1)


        'b. Fazla çamur (Px) miktarının hesaplanması

        'NOx : Nitrifikasyon bakteri biyokütlesi hesabında gerekli nitrata dönüşen amonyum azotu 'KABULLER bölümünde açıklanmıştır.

        Y = 0.45  'Metcalf eddy tablo 8-14
        Yn = 0.15 'Metcalf eddy tablo 8-14 ' nitrifikasyon bakterisi için gerekli verim sabiti
        fd = 0.15  'Metcalf eddy tablo 8-14

        'A=heteretrofik biyokütleden kaynaklı oluşan çamur
        'B=hücre yıkıntısından kaynaklı çamur
        'C=nitrifikasyon bakterisisinden kaynaklı çamur
        'A+B+C0 = TOPLAM ÇAMUR, Pxbiokabul

        A = (Flow * Y * (bCOD - S) / 1000) / (1 + TCbH * (TasarimSRT))

        B = ((fd * TCbH * Y * Flow * (bCOD - S) * TasarimSRT) / 1000 / (1 + TCbH * TasarimSRT))

        C0 = ((Flow * Yn * NOx) / 1000) / (1 + TCbAOB * TasarimSRT)

        Pxbiokabul = A + B + C0

        'PX,bio : Biyolojik fazla çamur kg/gün ' İçerisinde heteretrofik bakteri biyokütlesi, hücre yıkıntısı, nitrifikasyon bakteri biyokütlesi var
        'SRT:  Çamur yaşı, gün
        'bH : Heteretrofik bakteri için ozunma katsayısı, gün-1
        'bAOB : Nitrifikasyon bakterisi için için bozunma katsayısı, gün-1
        'Y : Verim sabiti (Dönüşüm oranı) g UAKM/g okside substrat
        'fd : Hücre yıkıntısı olarak kalan hücre kütle fraksiyonu, g/g
        'NOx : Nitrata dönüşmüş amonyum azotu, mg/L
        'S0 : Giriş bKOİ konsantrasyonu, mg/L
        'S:  Çıkış bKOİ, konsantrasyonu, mg / L


        '5) Nitrata dönüşen amonyum azotu miktarının hesaplanarak doğrulanması



        'NOx mikarının doğrulanması normalde (TKN * %80) olarak kabul edilmişti


        NOxdogru = TKNgiris - TNcikis - (0.12 * Pxbiokabul * 1000 / Flow)


        'yeni NOx değeri ile PxBio miktarı da tekrar hesaplanmalıdır.


        C = ((Flow * Yn * NOxdogru) / 1000) / (1 + TCbAOB * TasarimSRT)

        Pxbio = A + B + C
        '6) Fazla çamur miktarının hesaplanması


        'a. PXvss yükü miktarı

        Pxvss = Pxbio + (Flow * nbVSS / 1000)

        'b. PXtss yükü miktarı

        Pxtss = (Pxbio / 0.85) + (Flow * nbVSS / 1000) + Flow * iTSS / 1000
        fazlacamurtext.Text = Pxtss
        fazlacamurtext.Text = Math.Round(Val(fazlacamurtext.Text), 1)


        '7) Havalandırma (Nitrifikasyon) tank hacmi ve hidrolik bekletme süresinin hesaplanması

        'a. Tank hacminin hesaplanması

        If metanol = 0 Then
            MLSStesisici = CDbl(MLSStesisiçitext.Text)
        Else
            MLSStesisici = (Pxtss * TasarimSRT) / Voksik * 1000
        End If


        Voksik = (TasarimSRT * Pxtss / MLSStesisici) * 1000

        aerobiktanktext.Text = Voksik
        aerobiktanktext.Text = Math.Round(Val(aerobiktanktext.Text), 1)

        'b. Hidrolik bekletme süresinin hesaplanması

        HRT = Voksik * 24 / Flow  '24 ile saat/gün çevirimi yapılmıştır.



        'c. MLVSS miktarının hesaplanması

        MLVSS = (Pxvss / Pxtss) * MLSStesisici

        MLVSStext.Text = MLVSS
        MLVSStext.Text = Math.Round(Val(MLVSStext.Text), 1)

        'C. ANOKSİK TANK (DENİTRİFİKASYON) HESAPLARI

        '1) Aktif biyokütkle konsantrasyonunun hesaplanması

        Xb = ((Flow * TasarimSRT) / Voksik) * ((Y * (bCOD - S)) / (1 + TCbH * TasarimSRT))

        'Xb : Tanktaki Aktif biyokütle, mg/L
        'YH : Verim sabiti (Dönüşüm oranı) g UAKM/g okside substrat
        'bCOD (S0) : Giriş bKOİ konsantrasyonu , mg/L
        'S : Çıkış bKOİ konsantrasyonu , mg/L
        'TCbH : Heteretrofik bakteriler için bozunma katsayısı, gün-1, sıcaklık doğrulaması yapılmış
        'TasarımSRT : Nitrifikasyon için hesaplanan çamur yaşı, gün


        '2) İç geri devir(IR) oranının hesaplanması

        'NH4cikis = Sayfa3.Range("K13")  'tesisin sonundaki çıkış amonyum miktarı

        Ne = TNcikis - NH4cikis   'Ne: çıkışta istenilen nitrat miktarı

        ' R = Sayfa3.Range("G241")
        R = CDbl(Q_Qrtext.Text)
        IR = (NOxdogru / Ne) - 1 - R

        İcgeridevirtext.Text = IR
        İcgeridevirtext.Text = Math.Round(Val(İcgeridevirtext.Text), 1)

        'IR : İç geri devir oranı
        'NOx : Oksik tankta nitrifiye edilen amonyum azotu, mgNO3-N/L
        'Ne : Çıkış NO3-N konsantrasyonu, mg/L
        'TNçıkış : Toplam Azot çıkış konsantrasyonu, mg/L
        'NH4çkış : Çıkış Amonyum konsantrasyonu, mg/L
        'R : Çamur geri devir oranı, RAS ratio


        '3) Oksik tanktan anoksik tanka geçen nitrat yükünün hesaplanması


        flowtoanoksiktank = (IR * Flow + R * Flow) / 24
        QIRtext.Text = flowtoanoksiktank
        QIRtext.Text = Math.Round(Val(QIRtext.Text), 1)

        NOxfeed = (IR * Flow + R * Flow) * Ne / 1000



        'NOx (feed) : Oksik tanktan anoksik tanka beslenen Nitrat miktarı, kg/gün



        '4) Denitrifikasyon için gerekli anoksik tank hacminin hesaplanması
        Dim noxnew As Double

        noxnew = 100000
don:
        Vanoksik = Tankhacimorani * Voksik
        Anaerobiktantext.Text = Vanoksik
        Anaerobiktantext.Text = Math.Round(Val(Anaerobiktantext.Text), 1)


        '5) F/Mb oranının belirlenmesi

        F_Mb = Flow * BODgiris / (Vanoksik * Xb)


        'F/Mb : Besi maddesi mikroorganizma oranı, gBOD/g biyokütle.gün
        'Xb : Anoksik bölge biyokütle konsantrasyonu, mg/L
        'Vnox : Anoksik tank hacmi, m3

        '6) Standart Denitrifikasyon hızının (SDNR) hesaplanması


        If F_Mb > 0.5 And F_Mb < 1 Then


            rbCODfraksiyonu = (rbCOD / bCOD) * 100

            If rbCODfraksiyonu > 4.5 And rbCODfraksiyonu < 14.5 Then

                b0 = 0.186
                b1 = 0.078

            ElseIf rbCODfraksiyonu > 14.5 And rbCODfraksiyonu < 24.5 Then


                b0 = 0.213
                b1 = 0.118

            ElseIf rbCODfraksiyonu > 24.5 And rbCODfraksiyonu < 34.5 Then

                b0 = 0.235
                b1 = 0.141

            ElseIf rbCODfraksiyonu > 34.5 And rbCODfraksiyonu < 44.5 Then

                b0 = 0.242
                b1 = 0.152

            ElseIf rbCODfraksiyonu > 44.5 Then

                b0 = 0.27
                b1 = 0.162

            End If

            SDNRb = b0 + b1 * Math.Log(F_Mb)
            Dim ff As Double = Math.Exp(F_Mb)
            SDNRt = SDNRb * (1.026 ^ (Temp - 20))

        ElseIf F_Mb < 0.5 Then

            SDNRb = 0.24 * F_Mb
            SDNRt = SDNRb * (1.026 ^ (Temp - 20))
        ElseIf F_Mb >= 1 Then

            rbCODfraksiyonu = (rbCOD / bCOD) * 100


            If rbCODfraksiyonu > 4.5 And rbCODfraksiyonu < 14.5 Then

                b0 = 0.186
                b1 = 0.078

            ElseIf rbCODfraksiyonu > 14.5 And rbCODfraksiyonu < 24.5 Then


                b0 = 0.213
                b1 = 0.118

            ElseIf rbCODfraksiyonu > 24.5 And rbCODfraksiyonu < 34.5 Then


                b0 = 0.235
                b1 = 0.141

            ElseIf rbCODfraksiyonu > 34.5 And rbCODfraksiyonu < 44.5 Then

                b0 = 0.242
                b1 = 0.152

            ElseIf rbCODfraksiyonu > 44.5 Then


                b0 = 0.27
                b1 = 0.162

            End If

            SDNRb = b0 + b1 * Math.Log(F_Mb)

            SDNRt = SDNRb * (1.026 ^ (Temp - 20))

            SDNRadj = SDNRt - (0.029 * Math.Log(F_Mb)) - 0.012

            'SDNRt: Bu hesap eğer F/Mb oranı 1 den büyükse uygulanıyor değilse uygulamaya gerek yok, sıcaklık doğrulaması
            'SDNRadj:Bu hesap da eğer F/Mb oranı 1 den büyükse uygulanıyor, IR doğrulaması



            SDNRt = SDNRadj ' burada aşağıdaki formülde SDNRt kullanıldığı için SDNRadj olarak tanımlandı


        End If

        '7) SDNR değerinin aktif biyokütleye (Xb) göre hesaplanması OVERALL SDNR

        SDNR = SDNRt * Xb / MLVSS
        SDNRbirim = SDNR * 1000 / 24

        SDNRtext.Text = SDNRbirim
        SDNRtext.Text = Math.Round(Val(SDNRtext.Text), 1)



        'SDNR: Bu değer Metcalf eddy'de hesaplanmaktadır fakat kullanılmamamktadır.
        '8) Giderilmesi Hesaplanan potansiyel Nitrat yükünün hesaplanması

        NOr = Vanoksik * SDNRt * Xb / 1000

        'NOr: Hesaplara göre giderilebilecek nitrat yükü, g/gün


        'NOx çıkış niktarının hesabı





        NOxcikishesap = ((NOxfeed - NOr) * 1000) / Flow

        NO3cikistext.Text = NOxcikishesap
        NO3cikistext.Text = Math.Round(Val(NO3cikistext.Text), 1)



        ''' Bu döngü hacim oranını hesaplama döngüsüdür 

        Do Until NOxcikishesap > noxnew

            noxnew = NOxcikishesap

            Tankhacimorani = Tankhacimorani + 0.1

            GoTo don

        Loop

        Vratiotext.Text = Tankhacimorani - 0.1
        Tankhacimorani = CDbl(Vratiotext.Text)
        ''BURADA YUKARIDAKİİ KODLARIN BAZILARI TEKRAR ALINMIŞTIR ... üst sınır

        tankhacimdogrulamakodu() ' burada doğru tank hacminin tekrar koda girmesi sağlanıyor

        '9) Biyolojik arıtma için gerekli net oksijen miktarının hesaplanması


        OTR = ((Flow * (bCOD - S)) / 1000 - (1.42 * (A + B)) + (4.57 * NOxdogru * Flow) / 1000 - (2.86 * (NOxdogru - Ne) * Flow) / 1000) / 24



        oksijenihtiyacitext.Text = OTR
        oksijenihtiyacitext.Text = Math.Round(Val(oksijenihtiyacitext.Text), 1)

        'OTR : Gerçek oksijen ihtiyacı, kgO2/saat
        'bCOD : Giriş bKOİ konsantrasyonu , mg/L
        'S : Çıkış bKOİ konsantrasyonu , mg/L
        'Px , bioVSS: Fazla Çamur, kg / gün, (nitrifikasyon bakterileri hesaba katılmadı), O yüzden (A+B) olarak yazıldı
        'NN : Nitrifiye olacak amonyum azotu, kg/gün
        'NDN : Denitrifiye olacak nitrat azotu, kg/gün


        'SOTR hesabı için gerekli küçük hesaplamalar

        gravity = 9.81            'm/s^2
        rakim = CDbl(tesisrakimtext.Text)  'tesis rakım
        za = 0              'deniz seviyesi
        Rsabiti = 8314            '(kg*m^2)/(s^2*kg*K*mol)
        mol = 28.97         'kg/kg.mol
        Pb_Pa = Math.Exp(-1 * (gravity * mol * (rakim - za)) / (Rsabiti * (273.15 + Temp)))



        'Pb/Pa : H rakımındaki bağıl basınç oranı
        'gravity:  Yerçekimi kuvveti, M / s2
        'Rsabiti : Evrensel gaz sabiti, mol hava.Kelvin
        'M : Havanın molekül ağırlığı, g/g.mol, (28.94 g/g.mol)
        'rakım-za : Yükseklik farkı, m, (200m)
        'Pa : Deniz seviyesinde atmosfer basıncı, atm
        'Pb : H yüksekliğindeki atmosfer basıncı, atm
        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Cs20 = 9.09 '20 C ve 1 atm’de temiz suda çözünmüş oksijen doygunluk konsantrasyonu , Metcalf eddy appandix E-1

        De = 0.4 'Orta su seviyesi doğrulama katsayısı, tipik değeri 0,25-0,45
        Df = 0.5 'Difüzör derinliği
        Pa = 10.33 'Deniz seviyesinde atmosfer basıncı, atm
        Tankdepth = CDbl(tankdepthtext.Text)
        Cssonsuz20 = Cs20 * (1 + De * (Tankdepth - Df) / Pa)


        'Df: Difüzör yüksekliği, formülde bu değer tank yüksekliğinden çıkarılır
        'De : Orta su seviyesi doğrulama katsayısı, tipik değeri 0,25-0,45
        'Cssonsuz20: 20C sıcaklıkta ve H rakımında havalandırma tankında temiz sudaki çözünmüş oksijen ortalama doygunluk konsantrasyonu, mg/L."







        'alfa hesabı pöpel formülü

        alfa = 1 - (0.16 * (MLSStesisici / 1000) ^ (2 / 3))

        If rakim > 0 And rakim < 50 Then
            yukseklikvlook = 2
        ElseIf rakim > 50 And rakim < 300 Then
            yukseklikvlook = 3
        ElseIf rakim > 300 And rakim < 500 Then
            yukseklikvlook = 4
        ElseIf rakim > 500 And rakim < 700 Then
            yukseklikvlook = 5
        ElseIf rakim > 700 And rakim < 900 Then
            yukseklikvlook = 6
        ElseIf rakim > 900 And rakim < 1100 Then
            yukseklikvlook = 7
        ElseIf rakim > 1100 And rakim < 1300 Then
            yukseklikvlook = 8
        ElseIf rakim > 1300 And rakim < 1500 Then
            yukseklikvlook = 9
        ElseIf rakim > 1500 And rakim < 1700 Then
            yukseklikvlook = 10
        ElseIf rakim > 1700 And rakim < 1900 Then
            yukseklikvlook = 11

        End If

        'Dim excelTabloVerisi As Double = ExcelHelper.ExcelDosyaDizinindenOku(1, "A")
        'Dim excelTabloVerisi As Double = ExcelHelper.ExcelDosyaDizinindenOku(Temp, yukseklikvlook)  'ASIL KOD BU

        'Dim arananDeger As Double = ExcelApplication.WorksheetFunction.VLookup(25, worksheetSayfa1.Range("B39:L79"), 2, False)
        'Cst = Application.WorksheetFunction.VLookup(Temp, Sayfa2.Range("B39:L79"), yukseklikvlook, False)


        Cst = 8.063 'excelTabloVerisi  BAŞTAKİ 8.063 Ü SİL 

        Fouling = 0.9 'Tıkanma faktörü, tipik değeri 0.65- 0,9

        beta = 0.95  'Tuzluluk – yüzey gerilimi düzeltme faktörü, genellikle 0,95

        SOTR = (OTR / (alfa * Fouling)) * (Cssonsuz20 / (((beta * (Cst / Cs20) * Pb_Pa * Cssonsuz20 - disoxygen)))) * (1.024 ^ (20 - Temp))
        sotrtext.Text = SOTR
        sotrtext.Text = Math.Round(Val(sotrtext.Text), 1)

        'SOTR:Standart oksijen ihtiyacı, kg/saat

        '10) Biyolojik arıtma için gerekli hava miktarının hesaplanması


        SOTE = 28 ' BU DEĞER DE DEĞİŞEBİLİR

        'SOTE: Difüzör verimi, tesiste bulunan difüzörlere göre hesaplandı

        O2conc = 0.276   'kg O2/m3 ' metcalf eddy sayfa 1910 Appendix b-3 'barış hocayla hesaplayıp karar vermiştik alttaki 3 satırda açıklaması var ama 12 C ye ait

        '12oC ve belirtilen hava basıncında havanın yogunlugu 1.1633 kg/m3 alınabilir.
        'Belirtilen kosullarda hava içersindeki oksijenin ağırlıkça oranının 0,23 oldugu kabul edilirse;
        'hava içerisindeki oksijen konsantrasyonu; 0,23x1,1633 kg/m3 = 0,270  hava bulunacaktır.

        Havadebisi = SOTR / ((SOTE / 100) * O2conc) 'm^3 hava /saat
        blowerhavatext.Text = Havadebisi
        blowerhavatext.Text = Math.Round(Val(blowerhavatext.Text), 1)


        If NOxcikishesap > Ne Then
            'Use five arguments on the method.
            ' ... This asks a question and you can test the result using the variable.
            '
            Dim result3 As DialogResult = MsgBox("Karbon miktarı yeterli değildir metanol eklemesi yapmak için Metanol Ekle Butonuna basınız")

        End If







    End Sub

    Private Sub tankhacimdogrulamakodu()
        Vanoksik = Tankhacimorani * Voksik
        Anaerobiktantext.Text = Vanoksik
        Anaerobiktantext.Text = Math.Round(Val(Anaerobiktantext.Text), 1)


        '5) F/Mb oranının belirlenmesi

        F_Mb = Flow * BODgiris / (Vanoksik * Xb)


        'F/Mb : Besi maddesi mikroorganizma oranı, gBOD/g biyokütle.gün
        'Xb : Anoksik bölge biyokütle konsantrasyonu, mg/L
        'Vnox : Anoksik tank hacmi, m3

        '6) Standart Denitrifikasyon hızının (SDNR) hesaplanması


        If F_Mb > 0.5 And F_Mb < 1 Then


            rbCODfraksiyonu = (rbCOD / bCOD) * 100

            If rbCODfraksiyonu > 4.5 And rbCODfraksiyonu < 14.5 Then

                b0 = 0.186
                b1 = 0.078

            ElseIf rbCODfraksiyonu > 14.5 And rbCODfraksiyonu < 24.5 Then


                b0 = 0.213
                b1 = 0.118

            ElseIf rbCODfraksiyonu > 24.5 And rbCODfraksiyonu < 34.5 Then

                b0 = 0.235
                b1 = 0.141

            ElseIf rbCODfraksiyonu > 34.5 And rbCODfraksiyonu < 44.5 Then

                b0 = 0.242
                b1 = 0.152

            ElseIf rbCODfraksiyonu > 44.5 Then

                b0 = 0.27
                b1 = 0.162

            End If

            SDNRb = b0 + b1 * Math.Log(F_Mb)
            Dim ff As Double = Math.Exp(F_Mb)
            SDNRt = SDNRb * (1.026 ^ (Temp - 20))

        ElseIf F_Mb < 0.5 Then

            SDNRb = 0.24 * F_Mb
            SDNRt = SDNRb * (1.026 ^ (Temp - 20))
        ElseIf F_Mb >= 1 Then

            rbCODfraksiyonu = (rbCOD / bCOD) * 100


            If rbCODfraksiyonu > 4.5 And rbCODfraksiyonu < 14.5 Then

                b0 = 0.186
                b1 = 0.078

            ElseIf rbCODfraksiyonu > 14.5 And rbCODfraksiyonu < 24.5 Then


                b0 = 0.213
                b1 = 0.118

            ElseIf rbCODfraksiyonu > 24.5 And rbCODfraksiyonu < 34.5 Then


                b0 = 0.235
                b1 = 0.141

            ElseIf rbCODfraksiyonu > 34.5 And rbCODfraksiyonu < 44.5 Then

                b0 = 0.242
                b1 = 0.152

            ElseIf rbCODfraksiyonu > 44.5 Then


                b0 = 0.27
                b1 = 0.162

            End If

            SDNRb = b0 + b1 * Math.Log(F_Mb)

            SDNRt = SDNRb * (1.026 ^ (Temp - 20))

            SDNRadj = SDNRt - (0.029 * Math.Log(F_Mb)) - 0.012

            'SDNRt: Bu hesap eğer F/Mb oranı 1 den büyükse uygulanıyor değilse uygulamaya gerek yok, sıcaklık doğrulaması
            'SDNRadj:Bu hesap da eğer F/Mb oranı 1 den büyükse uygulanıyor, IR doğrulaması



            SDNRt = SDNRadj ' burada aşağıdaki formülde SDNRt kullanıldığı için SDNRadj olarak tanımlandı


        End If

        '7) SDNR değerinin aktif biyokütleye (Xb) göre hesaplanması OVERALL SDNR

        SDNR = SDNRt * Xb / MLVSS
        SDNRbirim = SDNR * 1000 / 24

        SDNRtext.Text = SDNRbirim
        SDNRtext.Text = Math.Round(Val(SDNRtext.Text), 1)



        'SDNR: Bu değer Metcalf eddy'de hesaplanmaktadır fakat kullanılmamamktadır.
        '8) Giderilmesi Hesaplanan potansiyel Nitrat yükünün hesaplanması

        NOr = Vanoksik * SDNRt * Xb / 1000

        'NOr: Hesaplara göre giderilebilecek nitrat yükü, g/gün


        'NOx çıkış niktarının hesabı





        NOxcikishesap = ((NOxfeed - NOr) * 1000) / Flow

        NO3cikistext.Text = NOxcikishesap
        NO3cikistext.Text = Math.Round(Val(NO3cikistext.Text), 1)
    End Sub
    '
    'Dim sonuc As String = yaz("muco", "cakirgoz")
    '''' <summary>
    '''' Ornek Fonksiyon
    '''' </summary>
    '''' <param name="isim"></param>
    '''' <param name="soyisim"></param>
    '''' <returns></returns>
    'Public Function yaz(isim As String, soyisim As String) As String

    '    Dim tumAd As String = isim + " " + soyisim

    '    Return tumAd
    'End Function












End Class
