# How to Translate application ?

1. Check out GitHub or Download File

2. You can use [Multilingual Toolkit](https://developer.microsoft.com/en-us/windows/downloads/multilingual-app-toolkit/) / [Pootle](https://pootle.translatehouse.org/) / [tinyTranslator](https://github.com/martinroob/tiny-translator)
3. see and translate [/Covid19Radar/MultilingualResources](https://github.com/Covid-19Radar/Covid19Radar/tree/master/Covid19Radar/Covid19Radar/MultilingualResources)

4. Translate and Commit (If you can use Git, Create branch and push)

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
| Covid19Radar.ar.xlf | Arabic |
| Covid19Radar.bn.xlf | Bangla |
| Covid19Radar.bg.xlf | Bulgarian |
| Covid19Radar.ca.xlf | Catalan	|
| Covid19Radar.zh-Hant.xlf | Chinese Traditional |
| Covid19Radar.zh-Hans.xlf | Chinese Simplified |
| Covid19Radar.hr.xlf | Croatian |
| Covid19Radar.cs.xlf | Czech |
| Covid19Radar.da.xlf | Danish |
| Covid19Radar.nl.xlf | Dutch |
| Covid19Radar.en.xlf | English |
| Covid19Radar.et.xlf | Estonian |
| Covid19Radar.fil.xlf | Filipino |
| Covid19Radar.fi.xlf | Finnish |
| Covid19Radar.fr.xlf | French |
| Covid19Radar.de.xlf | German |
| Covid19Radar.el.xlf | Greek |
| Covid19Radar.gu.xlf | Gujarati |
| Covid19Radar.he.xlf | Hebrew |
| Covid19Radar.hi.xlf | Hindi |
| Covid19Radar.hu.xlf | Hungarian |
| Covid19Radar.is.xlf | Icelandic |
| Covid19Radar.id.xlf | Indonesian |
| Covid19Radar.ga.xlf | Irish |
| Covid19Radar.it.xlf | Italian |
| Covid19Radar.ja.xlf | Japanese |
| Covid19Radar.kn.xlf | Kannada |
| Covid19Radar.sw.xlf | Kiswahili |
| Covid19Radar.ko.xlf | Korean |
| Covid19Radar.lv.xlf | Latvian |
| Covid19Radar.lt.xlf | Lithuanian |
| Covid19Radar.mg.xlf | Malagasy |
| Covid19Radar.ms.xlf | Malay |
| Covid19Radar.ml.xlf | Malayalam |
| Covid19Radar.mt.xlf | Maltese |
| Covid19Radar.mi.xlf | Maori |
| Covid19Radar.mr.xlf | Marathi |
| Covid19Radar.nb.xlf | Norwegian |
| Covid19Radar.fa.xlf | Persian |
| Covid19Radar.pl.xlf | Polish |
| Covid19Radar.pt.xlf | Portuguese |
| Covid19Radar.ro.xlf | Romanian |
| Covid19Radar.ru.xlf | Russian |
| Covid19Radar.sr-Cyrl.xlf | Serbian (Cyrillic) |
| Covid19Radar.sr-Latn.xlf | Serbian (Latin) |
| Covid19Radar.sk.xlf | Slovak |
| Covid19Radar.sl.xlf | Slovenian |
| Covid19Radar.es.xlf | Spanish |
| Covid19Radar.sv.xlf | Swedish |
| Covid19Radar.ta.xlf | Tamil |
| Covid19Radar.te.xlf | Telugu |
| Covid19Radar.th.xlf | Thai |
| Covid19Radar.tr.xlf | Turkish |
| Covid19Radar.uk.xlf | Ukrainian |
| Covid19Radar.vi.xlf | Vietnamese |
| Covid19Radar.ur.xlf | Urdu |
| Covid19Radar.cy.xlf | Welsh |
