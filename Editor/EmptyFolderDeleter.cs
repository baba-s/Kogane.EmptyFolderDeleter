using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kogane.Internal
{
    /// <summary>
    /// すべての空フォルダを削除するエディタ拡張
    /// </summary>
    internal static class EmptyFolderDeleter
    {
        //================================================================================
        // 定数
        //================================================================================
        private const string ITEM_NAME_ROOT = "Kogane/";

        //================================================================================
        // 関数（static）
        //================================================================================
        /// <summary>
        /// Unity メニューからすべての空フォルダを削除するための関数
        /// </summary>
        [MenuItem( ITEM_NAME_ROOT + "すべての空フォルダ削除" )]
        private static void DeleteFromMenu()
        {
            var isOk = EditorUtility.DisplayDialog
            (
                title: "",
                message: "すべての空フォルダを削除しますか？",
                ok: "OK",
                cancel: "Cancel"
            );

            if ( !isOk ) return;

            var result = Delete();

            if ( !result.Any() )
            {
                EditorUtility.DisplayDialog
                (
                    title: "",
                    message: "空フォルダは存在しませんでした",
                    ok: "OK"
                );

                return;
            }

            EditorUtility.DisplayDialog
            (
                title: "",
                message: "すべての空フォルダを削除しました\n削除したフォルダのパスはコンソールに出力されます",
                ok: "OK"
            );

            Debug.Log( $"以下の空フォルダを削除しました\n{string.Join( "\n", result )}" );
        }

        /// <summary>
        /// Assets フォルダと Packages フォルダ以下のすべての空フォルダを削除します
        /// </summary>
        private static string[] Delete()
        {
            var deleteAssets   = Delete( "Assets" );
            var deletePackages = Delete( "Packages" );

            return deleteAssets.Concat( deletePackages ).ToArray();
        }

        /// <summary>
        /// 指定されたフォルダ以下のすべての空フォルダを削除します
        /// </summary>
        private static IReadOnlyList<string> Delete( string path )
        {
            var list = new List<string>();

            foreach ( var x in GetList( path ) )
            {
                if ( x == ".github" ) continue;

                if ( Directory.Exists( x ) )
                {
                    Directory.Delete( x );
                }

                var metaPath = x + ".meta";

                if ( File.Exists( metaPath ) )
                {
                    File.Delete( metaPath );
                }

                list.Add( x );
            }

            return list;
        }

        /// <summary>
        /// 指定されたフォルダ以下のすべての空フォルダのパスを返します
        /// </summary>
        private static IEnumerable<string> GetList( string path )
        {
            foreach ( var dir in Directory.GetDirectories( path ).Select( x => x.Replace( "\\", "/" ) ) )
            {
                foreach ( var n in GetList( dir ) )
                {
                    yield return n;
                }

                var files = Directory
                        .GetFiles( dir )
                        .Where( x => !x.EndsWith( "/.DS_Store" ) )
                        .ToArray()
                    ;

                if ( files.Length != 0 ) continue;

                var dirs = Directory.GetDirectories( dir );

                if ( dirs.Length != 0 ) continue;

                yield return dir;
            }
        }
    }
}