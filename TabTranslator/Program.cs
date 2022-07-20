﻿using System;
using System.Linq;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TabTranslator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string path = @"..\..\..\..\JSONFiles";

            //Defining instrument

            MusicString GString0 = new MusicString();
            GString0.Tuning = RootNotes.E;
            MusicString GString1 = new MusicString();
            GString1.Tuning = RootNotes.B;
            MusicString GString2 = new MusicString();
            GString2.Tuning = RootNotes.G;
            MusicString GString3 = new MusicString();
            GString3.Tuning = RootNotes.D;
            MusicString GString4 = new MusicString();
            GString4.Tuning = RootNotes.A;
            MusicString GString5 = new MusicString();
            GString5.Tuning = RootNotes.E;

            List<MusicString> StandardTunings = new List<MusicString>(); //string list is reverse of normal to match json data
            StandardTunings.Add(GString0);
            StandardTunings.Add(GString1);
            StandardTunings.Add(GString2);
            StandardTunings.Add(GString3);
            StandardTunings.Add(GString4);
            StandardTunings.Add(GString5);

            StringInstrument SixStringGuitar = new StringInstrument();

            SixStringGuitar.Name = "AcousticGuitar";
            SixStringGuitar.FretCount = 30;
            SixStringGuitar.MusicStrings = StandardTunings;


            List<SongsterrSong> Songs = GetJsonSongs(path);

            List<MusicalBeat> songBeats = GetSongBeats(Songs[7], SixStringGuitar);

            // **TESTS**

            List<List<string>> TabLines = Tab.GetTabLines(Songs[7], SixStringGuitar);
            FillTablines(TabLines, songBeats, Songs[7]);


            //foreach (List<string> Tabline in FinalTab)
            //{
            //    foreach (string Measure in Tabline)
            //    {
            //        Console.Write(Measure);
            //    }
            //    Console.Write($"\n");
            //}
            //Console.ReadLine();

            List<string> tabOne = TabLines[0];
            int tabLength = tabOne.Count;
            int measuresPerLine = 10;
            int tabLineStartPoint = 0;
            int tabLineEndPoint = measuresPerLine;

            while (tabLineStartPoint < tabLength)
            {
                int remainingMeasures = tabLength - tabLineEndPoint;
                for (int i = 0; i < TabLines.Count; i++)
                {
                    List<string> tabLine = TabLines[i];
                    for (int h = tabLineStartPoint; h < tabLineEndPoint; h++)
                    {
                        string measure = tabLine[h];
                        int dashCount = measure.Length;

                        for (int k = 0; k < dashCount; k++)
                        {
                            Console.Write(measure[k]);
                        }
                    }
                    Console.Write($"\n");
                    //       File.AppendAllText(@"c:\test.txt", "lol");
                }
                tabLineStartPoint += 10;
                if (remainingMeasures >= 10)
                {
                    tabLineEndPoint += 10;
                }
                else
                {
                    tabLineEndPoint = tabLength;
                }
                Console.Write($"\n");
            }



        }
        /// <summary>
        /// inserts songnotes fretnumbers into tab structure
        /// </summary>
        /// <param name="TabLines"></param>
        /// <param name="Beats"></param>
        public static void FillTablines(List<List<string>> TabLines, List<MusicalBeat> Beats, SongsterrSong Song)
        {
            for (int idxTabLine = 0; idxTabLine < TabLines.Count; idxTabLine++)
            {
                List<string> TabLine = TabLines[idxTabLine];
                int beatCount = 0;
                for (int tabLineIndex = 1; tabLineIndex < TabLine.Count; tabLineIndex++)  //for each (output) measure
                {
                    int dashCount = 1;
                    while (dashCount < TabLine[tabLineIndex].Length)
                    {                      
                        // break if current noteCount exceeds actual count -> should not happen
                        if (beatCount >= Beats.Count - 1)
                        {
                            break;
                        }

                        // replace parts with current FretNr or skip if is rest
                        var currentBeat = Beats[beatCount];

                        for (int noteCount = 0; noteCount < currentBeat.MusicalNotes.Count; noteCount++)
                        {
                            var currentNote = currentBeat.MusicalNotes[noteCount];
                            if (currentNote.FingerPosition.FretNr != null && currentNote.FingerPosition.StringNum == idxTabLine)
                            {
                                TabLine[tabLineIndex] = TabLine[tabLineIndex].Remove(dashCount, 1);
                                TabLine[tabLineIndex] = TabLine[tabLineIndex].Insert(dashCount, currentNote.FingerPosition.FretNr.ToString());
                                //dashCount += Convert.ToInt32(currentBeat.Duration16ths);
                            }
                        }
                        //if (currentBeat.IsRest)
                        //{
                        //    dashCount += Convert.ToInt32(currentBeat.Duration16ths);
                        //}
                        dashCount += Convert.ToInt32(currentBeat.Duration16ths);
                        beatCount++;
                    }
                }
            }
        }
        /// <summary>
        /// Gets the notes, duration. and octave from songster json
        /// </summary>
        /// <param name="song"></param>
        /// <param name="stringInstrument"></param>
        /// <returns>list of MusicalNotes</returns>
        public static List<MusicalBeat> GetSongBeats(SongsterrSong song, StringInstrument stringInstrument)
        {

            List<MusicalBeat> beats = new List<MusicalBeat>();

            for (int measureNum = 0; measureNum < song.Measures.Count(); measureNum++)
            {

                for (int voiceNum = 0; voiceNum < song.Measures[measureNum].Voices.Count(); voiceNum++)
                {
                    for (int beatNum = 0; beatNum < song.Measures[measureNum].Voices[voiceNum].Beats.Count(); beatNum++)
                    {
                        MusicalBeat beat = new MusicalBeat();
                        List<MusicalNote> notes = new List<MusicalNote>();
                        beat.SongsterrDuration = song.Measures[measureNum].Voices[voiceNum].Beats[beatNum].Duration[1];
                        beat.Duration16ths = MusicalBeat.Get16ths(beat.SongsterrDuration);
                        beat.NullableBool = song.Measures[measureNum].Voices[voiceNum].Beats[beatNum].Rest;
                        beat.IsRest = MusicalBeat.GetRestBeat(beat.NullableBool);

                        for (int noteNum = 0; noteNum < song.Measures[measureNum].Voices[voiceNum].Beats[beatNum].Notes.Count(); noteNum++)
                        {
                            MusicalNote note = new MusicalNote();
                            note.FingerPosition.StringNum = song.Measures[measureNum].Voices[voiceNum].Beats[beatNum].Notes[noteNum].String;
                            note.FingerPosition.FretNr = song.Measures[measureNum].Voices[voiceNum].Beats[beatNum].Notes[noteNum].Fret;                          
                            note.RootNote = MusicalNote.GetRootNote(note.FingerPosition, stringInstrument.MusicStrings[Convert.ToInt32(note.FingerPosition.StringNum)]);
                            note.Octave = MusicalNote.GetOctave(note.FingerPosition);
                            note.NullableBool = song.Measures[measureNum].Voices[voiceNum].Beats[beatNum].Notes[noteNum].Rest;
                            note.IsRest = MusicalNote.GetRestNote(note.NullableBool);
                            notes.Add(note);
                        }
                        beat.MusicalNotes = notes;
                        beats.Add(beat);
                    }
                }
            }

            return beats;
        }

        /// <summary>
        /// Converts directory of json files into list of objects
        /// </summary>
        /// <param name="dPath"></param>
        /// <returns>List of objects</returns>
        public static List<SongsterrSong> GetJsonSongs(string dPath)
        {
            List<SongsterrSong> Songs = new List<SongsterrSong>();
            DirectoryInfo dir = new DirectoryInfo(dPath);
            string[] fPaths = Directory.GetFiles(dPath);
            string json = "";
            int fileNum = fPaths.Count();

            for (int i = 0; i < fileNum; i++)
            {
                json = File.ReadAllText(fPaths[i]);
                SongsterrSong song = JsonConvert.DeserializeObject<SongsterrSong>(json);
                Songs.Add(song);
            }

            return Songs;
        }

    }
}