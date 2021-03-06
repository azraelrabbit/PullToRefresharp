
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using SupportListFragment = Android.Support.V4.App.ListFragment;
using SupportFragmentManager = Android.Support.V4.App.FragmentManager;
using SupportFragmentTransaction = Android.Support.V4.App.FragmentTransaction;

using PullToRefresharp.Android.Views;
using PullToRefresharp.SwipeMenuList;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace Sample.Android.Fragments
{
    public class SampleListFragment : SupportListFragment, ISwipeMenuCreator, IOnMenuItemClickListener
    {
        // LOOK HERE!
        // The easiest way to interact with your pull to refresh view is to 
        // hold a reference to it that's typed to IPullToRefresharpView.
        private IPullToRefresharpView ptr_view;

        List<String> items = MainActivity.FLAVORS.ConvertAll<string>(x => (string)x.Clone());

        public bool FastScrollEnabled;

        public SampleListFragment() : base()
        {
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup root, Bundle data)
        {
            return inflater.Inflate(Resource.Layout.nav, null, false);
        }

        public override void OnViewStateRestored (Bundle savedInstanceState)
        {
            base.OnViewStateRestored (savedInstanceState);

            while (items.Count < 100) {
                items.AddRange(items);
            }

            var lv = ListView as PullToRefresharp.Android.Widget.ListView;
            if (lv != null)
            {
                lv.SetMenuCreator(this);
                lv.SetOnMenuItemClickListener(this);
            }

            ListView.FastScrollEnabled = FastScrollEnabled;

            if (ptr_view == null && ListView is IPullToRefresharpView) {
                ptr_view = (IPullToRefresharpView)ListView;
                // LOOK HERE!
                // Hookup a handler to the RefreshActivated event
                ptr_view.RefreshActivated += ptr_view_RefreshActivated;
            }
            ListAdapter = new ArrayAdapter(Activity, Resource.Layout.nav_item, items);
        }

        private void ptr_view_RefreshActivated(object sender, EventArgs args)
        {
            // LOOK HERE!
            // Refresh your content when PullToRefresharp informs you that a refresh is needed
            View.PostDelayed(() => {
                if (ptr_view != null) {
                    // When you are done refreshing your content, let PullToRefresharp know you're done.
                    ptr_view.OnRefreshCompleted();
                }
            }, 2000);
        }

        public override void OnDestroyView()
        {
            if (ptr_view != null) {
                ptr_view.RefreshActivated -= ptr_view_RefreshActivated;
                ptr_view = null;
            }

            base.OnDestroyView();
        }

        public override void OnResume ()
        {
            base.OnResume ();
            ListView.ItemClick += listview_ItemClick;
        }

        public override void OnPause ()
        {
            base.OnPause ();
            ListView.ItemClick -= listview_ItemClick;
        }

        private void listview_ItemClick(object sender, AdapterView.ItemClickEventArgs args)
        {
            Toast.MakeText(Activity, args.Position + " Clicked", ToastLength.Short).Show();
        }

        public void Create(SwipeMenu menu)
        {
            SwipeMenuItem item = new SwipeMenuItem(Activity.BaseContext);
            item.Title = "Item 1";
            item.Background = new ColorDrawable(Color.Gray);
			item.Width = Dp2Px (50);
            menu.AddMenuItem(item);

            item = new SwipeMenuItem(Activity.BaseContext);
            item.Title ="Item 2";
            item.Background = new ColorDrawable(Color.Red);
			item.Width = Dp2Px (50);
            menu.AddMenuItem(item);
        }

		private int Dp2Px(int dp)
		{
			return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp,
				Activity.BaseContext.Resources.DisplayMetrics);
		}

        public bool OnMenuItemClick(int position, SwipeMenu menu, int index)
        {
            Toast.MakeText(Activity.BaseContext, "The Position is " + position + " And Index Is" + index, ToastLength.Short).Show();
            return true;
        }
    }
}

