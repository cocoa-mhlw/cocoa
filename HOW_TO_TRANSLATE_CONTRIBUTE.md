# How to Translate application ?

1. Check out GitHub or Download File

2. You can use [Multilingual Toolkit](https://developer.microsoft.com/en-us/windows/downloads/multilingual-app-toolkit/) / [Pootle](https://pootle.translatehouse.org/) / [tinyTranslator](https://github.com/martinroob/tiny-translator)
3. see and translate [/Covid19Radar/MultilingualResources](https://github.com/Covid-19Radar/Covid19Radar/tree/master/Covid19Radar/Covid19Radar/MultilingualResources)

4. Translate and Commit (If you can use Git, Create branch and push, take care push XLF file only)

5. (2nd options) If you can't use Git , Could you add [issue on GitHub](https://github.com/Covid-19Radar/Covid19Radar/issues) and attach file.

## Notes on translation

After translation, make sure the target status change **needs-review-translation** to  **translated**.
Otherwise, it may be overwritten by machine translation.

```
        <trans-unit id="ButtonIWantToHelp" translate="yes" xml:space="preserve">
          <source>I want to help</source>
          <target state="needs-review-translation" state-qualifier="mt-suggestion">I want to help</target>
          <note from="MultilingualBuild" annotates="source" priority="2">I want to help</note>
        </trans-unit>
```
```
        <trans-unit id="ButtonIWantToHelp" translate="yes" xml:space="preserve">
          <source>I want to help</source>
          <target state="translated">協力する</target>
          <note from="MultilingualBuild" annotates="source" priority="2">I want to help</note>
        </trans-unit>
```

## MultilingualResources folder

 [/Covid19Radar/MultilingualResources](https://github.com/Covid-19Radar/Covid19Radar/tree/master/Covid19Radar/Covid19Radar/MultilingualResources)

| File  | Language  |
|---|---|
| Covid19Radar.af.xlf | Afrikaans |
| Covid19Radar.sq.xlf | Albanian |
| Covid19Radar.am.xlf | Amharic |
| Covid19Radar.ar.xlf | Arabic |
| Covid19Radar.hy.xlf | Armenian |
| Covid19Radar.az-Cyrl.xlf | Azerbaijani |
| Covid19Radar.bn.xlf | Bangla |
| Covid19Radar.eu.xlf | Basque |
| Covid19Radar.be.xlf | Belarusian |
| Covid19Radar.bs.xlf | Bosnian |
| Covid19Radar.bg.xlf | Bulgarian |
| Covid19Radar.ca.xlf | Catalan	|
| Covid19Radar.ceb.xlf | Cebuano |
| Covid19Radar.zh-Hant.xlf | Chinese Traditional |
| Covid19Radar.zh-Hans.xlf | Chinese Simplified |
| Covid19Radar.co.xlf | Corsican |
| Covid19Radar.hr.xlf | Croatian |
| Covid19Radar.cs.xlf | Czech |
| Covid19Radar.da.xlf | Danish |
| Covid19Radar.nl.xlf | Dutch |
| Covid19Radar.en.xlf | English |
| Covid19Radar.eo.xlf | Esperanto |
| Covid19Radar.et.xlf | Estonian |
| Covid19Radar.fil.xlf | Filipino |
| Covid19Radar.fi.xlf | Finnish |
| Covid19Radar.fr.xlf | French |
| Covid19Radar.fy.xlf | Frisian |
| Covid19Radar.gl.xlf | Galician |
| Covid19Radar.ka.xlf | Georgian |
| Covid19Radar.de.xlf | German |
| Covid19Radar.el.xlf | Greek |
| Covid19Radar.gu.xlf | Gujarati |
| Covid19Radar.ht.xlf | Haitian Creole |
| Covid19Radar.ha.xlf | Hausa |
| Covid19Radar.haw.xlf | Hawaiian |
| Covid19Radar.he.xlf | Hebrew |
| Covid19Radar.hi.xlf | Hindi |
| Covid19Radar.hmn.xlf | Hmong |
| Covid19Radar.hu.xlf | Hungarian |
| Covid19Radar.is.xlf | Icelandic |
| Covid19Radar.ig.xlf | Igbo |
| Covid19Radar.id.xlf | Indonesian |
| Covid19Radar.ga.xlf | Irish |
| Covid19Radar.it.xlf | Italian |
| Covid19Radar.ja.xlf | Japanese |
| Covid19Radar.jv.xlf | Javanese |
| Covid19Radar.kn.xlf | Kannada |
| Covid19Radar.kk.xlf | Kazakh |
| Covid19Radar.km.xlf | Khmer |
| Covid19Radar.rw.xlf | Kinyarwanda |
| Covid19Radar.sw.xlf | Kiswahili |
| Covid19Radar.ko.xlf | Korean |
| Covid19Radar.ku.xlf | Kurdish |
| Covid19Radar.ky.xlf | Kyrgyz |
| Covid19Radar.lo.xlf | Lao |
| Covid19Radar.la.xlf | Latin |
| Covid19Radar.lv.xlf | Latvian |
| Covid19Radar.lt.xlf | Lithuanian |
| Covid19Radar.lb.xlf | Luxembourgish |
| Covid19Radar.mk.xlf | Macedonian |
| Covid19Radar.mg.xlf | Malagasy |
| Covid19Radar.ms.xlf | Malay |
| Covid19Radar.ml.xlf | Malayalam |
| Covid19Radar.mt.xlf | Maltese |
| Covid19Radar.mi.xlf | Maori |
| Covid19Radar.mr.xlf | Marathi |
| Covid19Radar.mn.xlf | Mongolian |
| Covid19Radar.my.xlf | Myanmar (Burmese) |
| Covid19Radar.nb.xlf | Norwegian Bokmål |
| Covid19Radar.ny.xlf | Nyanja (Chichewa) |
| Covid19Radar.or.xlf | Odia (Oriya) |
| Covid19Radar.ps.xlf | Pashto |
| Covid19Radar.fa.xlf | Persian |
| Covid19Radar.pl.xlf | Polish |
| Covid19Radar.pt.xlf | Portuguese |
| Covid19Radar.pa.xlf | Punjabi |
| Covid19Radar.ro.xlf | Romanian |
| Covid19Radar.ru.xlf | Russian |
| Covid19Radar.sm.xlf | Samoan |
| Covid19Radar.gd.xlf | Scots Gaelic |
| Covid19Radar.sr-Cyrl.xlf | Serbian (Cyrillic) |
| Covid19Radar.sr-Latn.xlf | Serbian (Latin) |
| Covid19Radar.st.xlf | Sesotho |
| Covid19Radar.sn.xlf | Shona |
| Covid19Radar.sd.xlf | Sindhi |
| Covid19Radar.si.xlf | Sinhala (Sinhalese) |
| Covid19Radar.sk.xlf | Slovak |
| Covid19Radar.sl.xlf | Slovenian |
| Covid19Radar.so.xlf | Somali |
| Covid19Radar.es.xlf | Spanish |
| Covid19Radar.su.xlf | Sundanese |
| Covid19Radar.sv.xlf | Swedish |
| Covid19Radar.tl.xlf | Tagalog (Filipino) |
| Covid19Radar.ta.xlf | Tamil |
| Covid19Radar.te.xlf | Telugu |
| Covid19Radar.th.xlf | Thai |
| Covid19Radar.tg.xlf | Tajik |
| Covid19Radar.tt.xlf | Tatar |
| Covid19Radar.tr.xlf | Turkish |
| Covid19Radar.tk.xlf | Turkmen |
| Covid19Radar.uk.xlf | Ukrainian |
| Covid19Radar.ur.xlf | Urdu |
| Covid19Radar.ug.xlf | Uyghur |
| Covid19Radar.uz.xlf | Uzbek |
| Covid19Radar.vi.xlf | Vietnamese |
| Covid19Radar.cy.xlf | Welsh |
| Covid19Radar.xh.xlf | Xhosa |
| Covid19Radar.yi.xlf | Yiddish |
| Covid19Radar.yo.xlf | Yoruba |
| Covid19Radar.zu.xlf | Zulu |

