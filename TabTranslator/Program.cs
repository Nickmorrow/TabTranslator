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
            GString1.Tuning = RootNotes.A;
            MusicString GString2 = new MusicString();
            GString2.Tuning = RootNotes.D;
            MusicString GString3 = new MusicString();
            GString3.Tuning = RootNotes.G;
            MusicString GString4 = new MusicString();
            GString4.Tuning = RootNotes.B;
            MusicString GString5 = new MusicString();
            GString5.Tuning = RootNotes.E;

            List<MusicString>StandardTunings = new List<MusicString>();
            StandardTunings.Add(GString0);
            StandardTunings.Add(GString1);
            StandardTunings.Add(GString2);
            StandardTunings.Add(GString3);
            StandardTunings.Add(GString4);
            StandardTunings.Add(GString5);

            StringInstrument AcousticGuitar = new StringInstrument();

            AcousticGuitar.Name = "AcousticGuitar";
            AcousticGuitar.FretCount = 30;
            AcousticGuitar.MusicStrings = StandardTunings;
            

            List<SongsterrSong> Songs = GetJsonSongs(path);

            List<MusicalNote> songNotes = GetSongNotes(Songs[6], AcousticGuitar);

            for (int i = 0; i < songNotes.Count; i++)
            {

                Console.WriteLine($"{songNotes[i].FingerPosition.StringNum.ToString()}{songNotes[i].FingerPosition.FretNr.ToString()}{songNotes[i].RootNote.ToString()}");
            }
            
            

        } 

        public static List<MusicalNote> GetSongNotes(SongsterrSong song, StringInstrument stringInstrument)
        {
            
            List <MusicalNote>notes = new List<MusicalNote>();

            for (int i = 0; i < song.Measures.Count(); i++)
            {
                for (int j = 0; j < song.Measures[i].Voices.Count(); j++)
                {
                    for (int k = 0; k < song.Measures[i].Voices[j].Beats.Count(); k++)
                    {
                        for (int l = 0; l < song.Measures[i].Voices[j].Beats[k].Notes.Count(); l++)
                        {
                            MusicalNote note = new MusicalNote();
                            note.FingerPosition.StringNum = song.Measures[i].Voices[j].Beats[k].Notes[l].String;
                            note.FingerPosition.FretNr = song.Measures[i].Voices[j].Beats[k].Notes[l].Fret;
                            note.SongsterrDuration = song.Measures[i].Voices[j].Beats[k].Duration[1];
                            note.Duration16ths = MusicalNote.Get16ths(note.SongsterrDuration);
                            note.RootNote = MusicalNote.GetRootNote(note.FingerPosition, stringInstrument.MusicStrings[Convert.ToInt32(note.FingerPosition.StringNum)]);
                            notes.Add(note);
                        }
                    }
                }
            }

            return notes;
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